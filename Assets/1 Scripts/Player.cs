using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public int coin;
    public int stone; // 채집한 돌 개수
    float hAxis;
    float vAxis;
    bool rDown;
    bool jDown;
    bool iDown;
    bool sDown;
    bool tDown;
    bool isJump; 
    bool isCollision;
    public bool isShopping;
    public bool isTalking;
    public bool isInventory;
    public int[] hasItem;
    public GameObject selectItem; // 플레이어가 인벤토리에서 선택한 아이템

    public Shop shop;
    public Inventory inventory;

    Vector3 moveVec;
    GameObject nearObject;
    Rigidbody rigid;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Interaction();
    }
    // 입력
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction"); // E key
        sDown = Input.GetButtonDown("Submit"); // Enter or Space key
        tDown = Input.GetButtonDown("Inventory"); // Tab key
    }
    // 플레이어 이동
    void Move()
    {
        if (!isTalking && !isInventory)
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        else
            moveVec = new Vector3(0, 0, 0).normalized; // 대화 중인 경우
        
        // 물체 충돌 시 이동 제한
        if(!isCollision && !isTalking && !isInventory)
            transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", rDown);
    }
    // 플레이어 회전
    void Turn()
    {
        if(!isTalking && !isInventory)
            transform.LookAt(Vector3.MoveTowards(transform.position, transform.position + moveVec, Time.deltaTime));
    }
    // 점프
    void Jump()
    {
        if (jDown && !isJump && !isTalking && !isInventory) //npc근처에서 점프 금지
        {
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Interaction()
    {
        // 상점 상호작용
        // 
        // 상점 입장
        if (iDown && nearObject != null && !isJump && !isShopping && !isTalking && !isInventory)
        {
            if(nearObject.tag == "Shop")
            {
                shop.Enter(this);
            }
        }
        // 상점 입장 대사 넘김
        else if (sDown && !isShopping && isTalking)
        {
            if(shop.isNext)
            {
                shop.Close();
            }
        }
        // 상점 구매
        //
        // 상점 아이템 상호작용
        else if (iDown && isShopping && nearObject != null && nearObject.tag == "ShopItem" && !isTalking && !isInventory && !shop.isSell)
        {
            int index = nearObject.GetComponent<Item>().value;
            shop.Buy(index);
        }
        // 상점 아이템 구매 확인 대사 넘김
        else if (sDown && isShopping && isTalking && !shop.isSell)
        {
            // 구매 여부 선택지 이후 대사 넘김
            if (shop.isNext && shop.isClose && !shop.answerPanel.activeSelf)
                shop.Close();
            // 구매 선택지, 대사 닫기
            else if(shop.isNext && !shop.isClose && shop.answerPanel.activeSelf)
            {
                shop.isClose = true;
                shop.CloseAnswer();
            }
            // 선택지 표시
            else if (shop.isNext && !shop.answerPanel.activeSelf)
            {
                shop.ShowAnswer();
            }
        }
        // 상점 아이템 판매
        //
        // 판매 입장
        if (iDown && isShopping && nearObject != null && nearObject.tag == "Shop" && !isTalking && !isInventory)
        {
            shop.Sell();
        }
        else if (sDown && isShopping && isTalking && shop.isSell)
        {
            // 판매할 아이템 종류 선택 표시(돌맹이 or 인벤토리)
            if (!shop.isClose && shop.isNext)
            {
                shop.ShowSellAnswer();
            }
            // 아이템 판매 선택
            else if (!shop.isClose && !shop.isNext) {
                // 판매 아이템 종류 선택 시
                if(shop.sellChoosePanel.activeSelf)
                {
                    shop.CloseSellAnswer();
                }
                // 아이템 판매 개수 선택 시
                else if (!shop.sellCountPanel.activeSelf && !isInventory)
                {
                    shop.isNext = true;
                    shop.isClose = true;
                }
            }
            // 판매 종료
            else if (shop.isClose && shop.isNext && !shop.sellCountPanel.activeSelf)
            {
                shop.Close();
            }
        }

        // 인벤토리
        //
        // 플레이어 이야기 중인 경우
        if(isTalking)
        {
            inventory.btnInventory.SetActive(false);
        }
        // 인벤토리가 열리는 경우
        else if(!isTalking && !isInventory)
        {
            inventory.btnInventory.SetActive(true);
        }
        // tab 키 사용시 인벤토리 열기
        if(tDown && !isTalking && !isInventory)
        {
            inventory.ShowInventory();
        }
        // tab 키 사용시 인벤토리 닫기
        else if(tDown && !isTalking && inventory.panelInventroy.activeSelf)
        {
            isInventory = false;
            inventory.ShowBtn();
        }
        // 인벤토리 선택 버튼
        else if(sDown && isInventory && inventory.btnInventory.activeSelf)
        {
            isInventory = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isJump = false; // 점프 활성
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.position.y < transform.position.y)
        {
            anim.SetBool("isJump", false); // 점프 중지
        }

        if (other.gameObject.tag == "Ground")
        {
            anim.SetBool("isJump", false); // 점프 중지
        }
        else if(other.gameObject.tag != "Shop" && other.gameObject.tag != "Shopping" && other.gameObject.tag != "ShopItem")
            isCollision = true; // 맵에 충돌 중
    }

    private void OnTriggerStay(Collider other)
    {
        // 상점 출입, 판매 지점 인식
        if (other.tag == "Shop")
        {
            nearObject = other.gameObject;
            if (!isTalking)
                shop.SetEPosition(other);
            else
                shop.showKeyE.SetActive(false);
        }
        // 상점 아이템 상호작용 가능 여부 인식
        else if(other.tag == "ShopItem" && isShopping)
        {
            nearObject = other.gameObject;
            if (!isTalking)
                shop.SetEPosition(other);
            else
                shop.showKeyE.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isCollision = false;
        // 상점 지역에서 빠져나오는 경우
        if(other.tag == "Shopping" && isShopping)
        {
            isShopping = false;
            shop.Exit();
            nearObject = null;
        }
        // 상점 이용 시 주변에 상호작응 가능한 아이템 없는 경우
        else if ((other.tag == "ShopItem" || other.tag == "Shop") && nearObject != null)
        {
            nearObject = null;
            shop.showKeyE.SetActive(false);
        }
    }
}