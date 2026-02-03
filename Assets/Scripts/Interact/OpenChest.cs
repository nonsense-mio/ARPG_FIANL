using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
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
            //如果字典里没有该宝箱ID 则添加进去 并设置为未开启状态
            if (!CurrentGameDataMgr.Instance.playerData.chestDic.ContainsKey(chestID))
            {
                CurrentGameDataMgr.Instance.playerData.chestDic.Add(chestID, false);
            }
            hasOpened = CurrentGameDataMgr.Instance.playerData.chestDic[chestID];
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
            //开启协程 生成宝箱里的物品
            StartCoroutine(SpawnItemInChest());
            if (CurrentGameDataMgr.Instance.playerData.chestDic.ContainsKey(chestID))
            {
                CurrentGameDataMgr.Instance.playerData.chestDic.Remove(chestID);
            }
            CurrentGameDataMgr.Instance.playerData.chestDic.Add(chestID, true);
            hasOpened = true;

        }

        private IEnumerator SpawnItemInChest()
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < itemsInChest.Count; i++)
            {
                GameObject obj = PoolMgr.Instance.GetObj(itemSpawnerName);
                obj.transform.position = transform.position;
                ItemPickUp itemPickUp = obj.GetComponent<ItemPickUp>();
                if (itemPickUp != null)
                {
                    itemPickUp.item = itemsInChest[i];
                }
            }

            Destroy(this);
        }

    }

}
