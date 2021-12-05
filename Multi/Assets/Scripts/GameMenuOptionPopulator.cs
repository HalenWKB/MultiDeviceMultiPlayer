using System.Collections;
using System.Collections.Generic;
using MultiplayerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameMenuOptionPopulator : MonoBehaviour
{
    [SerializeField] private GameTypeDictionary m_gameDict = null;
    
    [SerializeField] private Transform m_singlePlayerStartItems = null;
    [SerializeField] private int m_minOptionCountForSinglePlayerRoomTypes = 4;
    
    [SerializeField] private Transform m_multiplayerRoomTypes = null;
    [SerializeField] private int m_minOptionCountForMultiplayerRoomTypes = 6;

    [SerializeField] private GameObject m_menuOptionPrefab = null;
    [SerializeField] private GameObject m_menuBlankPrefab = null;
    [SerializeField] private GameObject m_menuSelectHighlightPrefab = null;

    [SerializeField] private MainMainMenu m_mainMenu = null;
    [SerializeField] private LobbyController m_lobbyController = null;
    
    
    void Start()
    {
        int listItemCount = 0;
        for (int i = 0; i < m_gameDict.Games.Count; i++)
        {
            GameTypeDetails gtd = m_gameDict.Games[i];
            if (gtd.active)
            {
                listItemCount++;
                CreateOption(gtd, false);
                CreateOption(gtd, true, i == 0);
            }
        }

        //Move 'back' button to bottom of list...
        m_singlePlayerStartItems.GetChild(0).SetSiblingIndex(m_singlePlayerStartItems.childCount - 1);
        
        FillBlanks(m_singlePlayerStartItems, m_minOptionCountForSinglePlayerRoomTypes - listItemCount - 1);
        FillBlanks(m_multiplayerRoomTypes, m_minOptionCountForMultiplayerRoomTypes - listItemCount);
    }

    void FillBlanks(Transform parent, int blankCount)
    {
        for (int i = 0; i < blankCount; i++)
        {
            Instantiate(m_menuBlankPrefab, parent);
        }
    }

    void CreateOption(GameTypeDetails gtd, bool multiplayer, bool startSelected = false)
    {
        GameObject option = Instantiate(m_menuOptionPrefab, multiplayer 
            ? m_multiplayerRoomTypes 
            : m_singlePlayerStartItems);
        option.GetComponentInChildren<TextMeshProUGUI>().text = gtd.displayName;
        
        if (multiplayer)
        {
            GameObject optionHighlight = Instantiate(m_menuSelectHighlightPrefab, option.transform);
            optionHighlight.SetActive(false);
            option.GetComponent<Button>().onClick.AddListener(GetMultiplayerButtonEvent(gtd, optionHighlight));
            if (startSelected) 
                SelectRoomType(gtd, optionHighlight); 
        }
        else
            option.GetComponent<Button>().onClick.AddListener(GetSinglePlayerButtonEvent(gtd));
    }

    UnityAction GetSinglePlayerButtonEvent(GameTypeDetails gtd)
    {
        return () => m_mainMenu.PlaySinglePlayerGame(gtd);
    }
    
    UnityAction GetMultiplayerButtonEvent(GameTypeDetails gtd, GameObject highlight)
    {
        return () => SelectRoomType(gtd, highlight);
    }

    void SelectRoomType(GameTypeDetails gtd, GameObject highlight)
    {
        m_lobbyController.OnRoomTypeSelected(gtd, highlight);
    }
}
