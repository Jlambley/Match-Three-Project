using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public Animator shopAnim;
    public GameObject shopPanel, equipmentPanel;

    public Sprite Trex, Tri, Iguana;
    private GameData data;
    public GameObject outofstock1, outofstock2, outofstock3;

    [Header("Confirm Purchase")]

    public GameObject purchasePanel;
    public GameObject  item, payment;
    public int price;
    public Text cost, confirm;
    

    [Header("Sprite Library")]

    public Sprite ruby;
    public Sprite emerald, diamond;
    // Start is called before the first frame update

    public void Start()
    {
        data = FindObjectOfType<GameData>();
        CheckPurchases();
    }

    public void ShowPurchase()
    {
        purchasePanel.SetActive(true);

    }

    public void SelectPurchase(int id)
    {
       
        ShowPurchase();
        switch (id)
        {
            
            case 1: item.GetComponent<Image>().sprite = Trex; payment.GetComponent<Image>().sprite = diamond; price = 15; break;  //item = icon, price = icon, cost = 0000
            case 2: item.GetComponent<Image>().sprite = Tri; payment.GetComponent<Image>().sprite = emerald; price = 10; break;
            case 3: item.GetComponent<Image>().sprite = Iguana; payment.GetComponent<Image>().sprite = ruby; price = 5; break;
               
        }
        UpdateCostText();
    }

    private void UpdateCostText()
    {
        cost.text = "-" + price;

    }

    public void BuyPurchase()
    {
        switch (price)
        {
            case 5: if (data.saveData.bones[10] < 5)
                {
                    StartCoroutine(NotEnoughFunds()); break;
                    
                }else
                {
                    BuyItem3(); CancelPurchase();
                    break;
                }
            case 10:
                if (data.saveData.bones[11] < 10)
                {
                    StartCoroutine(NotEnoughFunds()); break;

                }
                else
                {
                    BuyItem2(); CancelPurchase();
                    break;
                }
            case 15:
                if (data.saveData.bones[11] < 15)
                {
                    StartCoroutine(NotEnoughFunds()); break;

                }
                else
                {
                    BuyItem1(); CancelPurchase();
                    break;
                }
        }
        
        
    }

    private void CheckPurchases()
    {
        if (PlayerPrefs.HasKey("UnlockedTrex"))
        {
            outofstock3.SetActive(true);
        }
        if (PlayerPrefs.HasKey("UnlockedTricera"))
        {
            outofstock2.SetActive(true);
        }
        if (PlayerPrefs.HasKey("UnlockedIguana"))
        {
            outofstock1.SetActive(true);
        }
    }

    private void BuyItem1()
    {
        PlayerPrefs.SetInt("UnlockedTrex", 1);
        data.saveData.bones[10] = data.saveData.bones[10] - 5;


    }
    private void BuyItem2()
    {
        PlayerPrefs.SetInt("UnlockedTricera", 1);
        data.saveData.bones[11] = data.saveData.bones[11] - 10;

    }
    private void BuyItem3()
    {
        PlayerPrefs.SetInt("UnlockedIguana", 1);
        data.saveData.bones[12] = data.saveData.bones[12] - 15;

    }

    public void ClearItems()
    {
        PlayerPrefs.DeleteKey("UnlockedTricera");
        PlayerPrefs.DeleteKey("UnlockedTrex");
        PlayerPrefs.DeleteKey("UnlockedIguana");
        PlayerPrefs.DeleteKey("Equipment");
    }

    public IEnumerator NotEnoughFunds()
    {

        confirm.text = "Not Enough Funds!";
        yield return new WaitForSeconds(1f);
        confirm.text = "Confirm Purchase?";
    }

    public void CancelPurchase()
    {
        CheckPurchases();
        purchasePanel.SetActive(false);
    }

    public void CloseShop()

    {
        shopAnim.SetTrigger("Out");

    }

    public void ShowShop()
    {
        if(shopPanel.activeInHierarchy)
        {
            shopAnim.SetTrigger("In");
        }

        shopPanel.SetActive(true);
    }

    public void ShowEquipment()
    {
        if (equipmentPanel.activeInHierarchy == true)
        {
            equipmentPanel.SetActive(false);
        }
        else if (equipmentPanel.activeInHierarchy == false)
        {

            equipmentPanel.SetActive(true);
        }
    }
}
