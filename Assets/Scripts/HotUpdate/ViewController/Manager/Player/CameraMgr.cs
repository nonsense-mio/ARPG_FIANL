using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class CameraMgr : MonoBehaviour
    {
        public InputMgr inputMgr;
        public PlayerManager playerManager;

        public Transform targetTransform; //摄像机跟随的目标
        public Transform targetTransformWhileAiming;//瞄准时摄像机跟随的目标
        public Transform cameraTransform;
        public Camera cameraObj;
        public Transform cameraPivotTransform;
        private Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;
        public LayerMask environmentLayer;
        private Vector3 cameraFollowVelocity = Vector3.zero;


        public float horizontalSpeed = 250f;
        public float horizontalAimingSpeed = 25f;
        public float followSpeed = 1f;
        public float airFollowSpeed = 1f;
        public float verticalSpeed = 250f;
        public float verticalAimingSpeed = 25f;

        public float targetPosition;
        private float defaultPostion;
        private float horizontalAngle;
        private float verticalAngle;
        public float minimumVerticalAngle = -35;
        public float maximumVerticalAngle = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        // 锁定视角近距离保护
        public float minimumHorizontalDistance = 2f;    // 近距离时水平距离底线，防止仰角过大
        public float maximumLockedVerticalAngle = 30f;  // 锁定时最大俯角（向下看）
        public float minimumLockedVerticalAngle = -30f; // 锁定时最大仰角（向上看）
        public float lockedPivotRotationSpeed = 5f;     // pivot旋转平滑速度

        public CharacterManager currentLockOnTarget;

        List<CharacterManager> availableTargetList = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;
        public float maximumLockOnDistance = 30;
        private void Awake()
        {
            defaultPostion = cameraTransform.position.z;
            //忽略 8、9、10层
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            // inputMgr = FindObjectOfType<InputMgr>();
            // playerManager = FindObjectOfType<PlayerManager>();
            cameraObj = GetComponentInChildren<Camera>();
        }
        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }
        /// <summary>
        /// 摄像机跟随目标的方法
        /// </summary>
        /// <param name="delta"></param>
        public void FollowTarget()
        {
            if (playerManager.isAiming)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetTransformWhileAiming.position, ref cameraFollowVelocity, Time.deltaTime * followSpeed);
            }
            else
            {
                if (playerManager.isGrounded)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, Time.deltaTime * followSpeed);
                }
                else
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, Time.deltaTime * airFollowSpeed);
                }

            }
            HandleCameraCollision();

        }

        //摄像机旋转方法
        public void HandleCameraRotation()
        {
            if (inputMgr.lockOnFlag && currentLockOnTarget != null)
            {
                HandleLockedCameraRotation();
            }
            else if (playerManager.isAiming)
            {
                HandleAimingCameraRotation();
            }
            else
            {
                HandleStandardCameraRotation();
            }
        }
        //视角锁定时摄像机的旋转方法
        private void HandleLockedCameraRotation()
        {
    
            //处理摄像机的水平旋转
            Vector3 dir = currentLockOnTarget.transform.position - base.transform.position;
            dir.Normalize();
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            //摄像机 始终朝向锁定目标
            transform.rotation = targetRotation;

            //调整摄像机枢轴的垂直旋转
            dir = currentLockOnTarget.transform.position - cameraPivotTransform.position;

            // 分离水平（XZ）和垂直（Y）分量
            Vector3 flatDir = new(dir.x, 0, dir.z);
            float horizontalDist = flatDir.magnitude;
            float verticalDiff = dir.y;

            // 关键修复：使用水平距离底线，防止近距离时仰角趋近于90°
            float effectiveHorizontalDist = Mathf.Max(horizontalDist, minimumHorizontalDistance);

            // 用 Atan2 计算俯仰角（Unity X正=俯，X负=仰，故取负号）
            float pitchAngle = -Mathf.Atan2(verticalDiff, effectiveHorizontalDist) * Mathf.Rad2Deg;

            // 硬角度限制（双重保险）
            pitchAngle = Mathf.Clamp(pitchAngle, minimumLockedVerticalAngle, maximumLockedVerticalAngle);

            // 平滑过渡，防止角度跳变
            Quaternion targetPivotRotation = Quaternion.Euler(pitchAngle, 0, 0);
            cameraPivotTransform.localRotation = Quaternion.Slerp(
                cameraPivotTransform.localRotation,
                targetPivotRotation,
                Time.deltaTime * lockedPivotRotationSpeed
            );
        }

        //瞄准时摄像机的旋转方法
        private void HandleAimingCameraRotation()
        {
            transform.rotation = Quaternion.identity;
            cameraPivotTransform.localRotation = Quaternion.identity;

            Quaternion targetRotationX;
            Quaternion targetRotationY;

            Vector3 cameraRotationX = Vector3.zero;
            Vector3 cameraRotationY = Vector3.zero;

            horizontalAngle += inputMgr.mouseX * horizontalAimingSpeed * Time.deltaTime;
            verticalAngle -= inputMgr.mouseY * verticalAimingSpeed * Time.deltaTime;

            cameraRotationY.y = horizontalAngle;
            targetRotationY = Quaternion.Euler(cameraRotationY);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationY, 1);

            cameraRotationX.x = verticalAngle;
            targetRotationX = Quaternion.Euler(cameraRotationX);
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, targetRotationX, 1);

        }
        public void ResetAimCameraRotation()
        {
            cameraTransform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 标准情况下摄像机旋转的方法
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="mouseXInput">鼠标在水平方向上的偏移</param>
        /// <param name="mouseYInput">鼠标在垂直方向上的偏移</param>
        public void HandleStandardCameraRotation()
        {
            //在水平方向上的旋转
            horizontalAngle += inputMgr.mouseX * horizontalSpeed * Time.deltaTime;
            //在垂直方向上的旋转
            verticalAngle -= inputMgr.mouseY * verticalSpeed * Time.deltaTime;
            //垂直方向上旋转角度的限制
            verticalAngle = Mathf.Clamp(verticalAngle, minimumVerticalAngle, maximumVerticalAngle);

            //水平旋转的处理
            Vector3 rotation = Vector3.zero;
            //将水平旋转角度赋值给y轴
            rotation.y = horizontalAngle;
            //将旋转角度转换为四元数
            Quaternion targetRotation = Quaternion.Euler(rotation);
            //将旋转量赋值给摄像机
            transform.rotation = targetRotation;

            //垂直旋转的处理
            rotation = Vector3.zero;
            //将垂直旋转角度赋值给x轴
            rotation.x = verticalAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        /// <summary>
        /// 处理摄像机碰撞的方法
        /// </summary>
        /// <param name="delta"></param>
        private void HandleCameraCollision()
        {   //defaultPostion为摄像机z轴默认位置
            targetPosition = defaultPostion;
            RaycastHit hit;
            //这里是从枢轴 指向 摄像机的 向量
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();
            //碰撞检测 以摄像机枢轴为圆心 
            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                Mathf.Abs(targetPosition),
                ignoreLayers))
            {
                //Debug.Log("摄像机发生碰撞" + hit.collider.name);
                //摄像机枢轴位置到碰撞点位置的距离
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                //碰撞后的位置计算  如果发生碰撞要将摄像机的位置调整到目标位置
                targetPosition = -(dis - cameraCollisionOffset);
            }
            //利用插值平滑地调整摄像机在局部空间中的z轴位置
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, Time.deltaTime / 0.2f);
            //将位置赋值给摄像机
            cameraTransform.localPosition = cameraTransformPosition;
        }
        /// <summary>
        /// 锁定视角相关
        /// </summary>
        public void HandleLockOn()
        {
            availableTargetList.Clear();
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            //ClearLockOnTargets();
            //范围检测 
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, maximumLockOnDistance, 1 << LayerMask.NameToLayer("Character"));

            for (int i = 0; i < colliders.Length; i++)
            {
                //找到人物对象 
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;
                    //当对象不是玩家本身且 摄像机与锁定目标角度小于一定值时 且 且玩家与目标的距离没超过限定值时
                    if (character.transform.root != targetTransform.transform.root &&
                       viewableAngle > -50 && viewableAngle < 50 &&
                       distanceFromTarget <= maximumLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);
                            if (hit.transform.gameObject.layer == environmentLayer)
                            {
                                //不能锁定视角 有其它物品阻挡
                            }
                            else
                            {
                                //把目标对象添加到可锁定的列表中
                                availableTargetList.Add(character);
                            }
                        }

                    }
                }
            }
            //遍历目标列表 找出其中与玩家最近的目标 
            for (int i = 0; i < availableTargetList.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargetList[i].transform.position);
                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargetList[i];
                }

                if (inputMgr.lockOnFlag)
                {
                    Vector3 relativeEnemyPosition = inputMgr.transform.InverseTransformPoint(availableTargetList[i].transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.x;

                    if (relativeEnemyPosition.x <= 0.00 &&
                        distanceFromLeftTarget > shortestDistanceOfLeftTarget &&
                        availableTargetList[i] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargetList[i];
                    }
                    else if (relativeEnemyPosition.x >= 0.00 &&
                       distanceFromRightTarget < shortestDistanceOfRightTarget &&
                       availableTargetList[i] != currentLockOnTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargetList[i];
                    }
                }
            }
        }
        /// <summary>
        /// 清除锁定目标
        /// </summary>
        public void ClearLockOnTargets()
        {
            availableTargetList.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
            leftLockTarget = null;
            rightLockTarget = null;
        }

        /// <summary>
        /// 主要用于锁定视角时调整摄像机的高度
        /// </summary>
        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if (currentLockOnTarget != null)
            {
                cameraPivotTransform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }

}
