using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Deco : MonoBehaviour
{
    //������ �Ǽ�
    public GameObject Decoration; //����Ʈ �����Ҷ� �ٹ̱� ���Ȱ��ȭ
    public GameObject[] decoObj; //�ٹ̱� ������ ����
    public Transform decoPos; //�ٹ̱� ������ ��ġ ���
    public GameObject decoZone; //��ġ ��� ǥ��


    public GameObject showKey; //������ ���� tabŰ, ��ġ eŰ
    public Text showKeyText;
    public int itemIndex;

    public bool nearDecoZone;

    public Player player;
    public Inventory inventory;
    public Quest quest;



    void Update()
    {
        if (player.selectItem != null && nearDecoZone)
            showKeyText.text = "EŰ�� ���� ������ ���������� ������ �ٹ̱�";

        if (Input.GetKeyDown(KeyCode.E) && nearDecoZone) //�ٹ̱� ��ư
        {
            SelectDeco();
            showKey.SetActive(false);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            nearDecoZone = true;
            showKeyText.text = "TabŰ�� �κ��丮�� ���� ������ �����ϱ�";
            showKey.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            nearDecoZone = false;
            showKey.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player.isInventory)
            {
                showKey.SetActive(false);
            }
            else
                showKey.SetActive(true);

        }
    }

    private void SelectDeco()
    {   
        if(player.selectItem != null) //�������� ���õǸ�
        {
            // ���� ������ �ε���
            int index = player.selectItem.GetComponent<Item>().value; //itemIndex = index;
            itemIndex = index;

            if (itemIndex >= 0) //�� ����
            {
                player.hasItem[itemIndex] -= 1;
                player.selectItem = null;

                Decorate();

                //������ �ı�, ����Ʈ ���൵
                quest.thirdQuest += 1;
                decoZone.SetActive(false);
                //Destroy(decoZone); �� destroy�� ���� ����Ʈ�� �Ѿ�� ������..?
            }
        }
    }
    private void Decorate()
    {
        //��ġ, ũ�� ����
        GameObject newObject = Instantiate(decoObj[itemIndex],decoPos.position, decoPos.rotation);
        newObject.transform.localScale = new Vector3(2, 2, 2);

    }




}