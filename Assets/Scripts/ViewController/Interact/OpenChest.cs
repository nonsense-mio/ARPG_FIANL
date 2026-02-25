using System.Collections.Generic;
using UnityEngine;
using Framework;
using ARPG;

namespace ARPG
{
    public class OpenChest : Interactable
    {
        [SerializeField] string chestID;
        [SerializeField] bool hasOpened = false;

        public Transform top;
        Animator animator;

        public Transform playerStandingPostion;
        public GameObject itemSpawner;
        public string itemSpawnerName;
        public List<Item_SO> itemsInChest;

        protected override void Awake()
        {
            base.Awake();
            interactionPrompt = "打开宝箱";
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // 从 SceneStateModel 读取宝箱状态
            var sceneStateModel = this.GetModel<ISceneStateModel>();
            hasOpened = sceneStateModel.IsChestOpened(chestID);

            if (hasOpened)
            {
                animator.enabled = false;
                top.transform.localEulerAngles = new Vector3(-110, 0, 0);
                Destroy(this);
                return;
            }
        }

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            //玩家转向宝箱
            Vector3 rotationDirection = transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();
            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            playerManager.transform.rotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            //宝箱打开
            animator.Play("Open Chest");
            playerManager.OpenChestInteraction(playerStandingPostion);
            //延迟生成宝箱里的物品（等待开箱动画）
            this.GetSystem<ITimerSystem>().CreateTimer(false, 1f, () =>
            {
                for (int i = 0; i < itemsInChest.Count; i++)
                {
                    GameObject obj = this.GetSystem<IPoolSystem>().Spawn(itemSpawnerName, transform.position, Quaternion.identity);
                    ItemPickUp itemPickUp = obj.GetComponent<ItemPickUp>();
                    if (itemPickUp != null)
                        itemPickUp.item = itemsInChest[i];
                }
                Destroy(this);
            });

            // 更新 SceneStateModel 宝箱状态
            var sceneStateModel = this.GetModel<ISceneStateModel>();
            sceneStateModel.SetChestOpened(chestID, true);
            hasOpened = true;
        }

    }

}
