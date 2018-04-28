using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {

    #region GUI_ELEMENTS
    public Text playerNameText;
    public RectTransform healthBar;
    public Text healthText;
    public RectTransform mpBar;
    public Text mpText;
    public SpellInterface[] spells;
    #endregion // GUI_ELEMENTS

    private Player player;
    private MatchController controller;

    public void SetMatchController(MatchController controller) {
        this.controller = controller;
    }
    public void SetPlayer(Player player) {
        this.player = player;
        for (int i = 0; i < 4; ++i) {
            //spells[i].spellImage.sprite = Resources.Load<Sprite>(player.spells[i].iconPath);
        }
    }

    public void UpdateInterface() {
        UpdateHealthBar();
        UpdateMpBar();
        UpdateSpellBar();
    }
    public void UpdateHealthBar() {
        float percent = (float)player.healthCurrent / (float)player.healthMax;
        healthBar.localScale = new Vector3(percent, 1, 1);
        healthText.text = player.healthCurrent + "/" + player.healthMax;
    }
    public void UpdateMpBar() {
        float percent = (float)player.mpCurrent / (float)player.mpMax;
        mpBar.localScale = new Vector3(percent, 1, 1);
        mpText.text = player.mpCurrent + "/" + player.mpMax;
    }
    public void UpdateSpellBar() {
        for (int i = 0; i < 4; ++i) {
            //if(player.spells[i].isRecharging) {
            //    spells[i].spellText = player.spells[i].cooldown;
            //    spells[i].spellText.enabled = true;
            //    spells[i].spellButton.interactable = false;
            //} else {
            //    spells[i].spellText.enabled = false;
            //    spells[i].spellButton.interactable = true;
            //}
        }
    }

    public void SetPanelInteractable(bool value) {
        for (int i = 0; i < 4; ++i) {
            //spells[i].spellButton.interactable = value && player.spells[i].isRecharging;
        }
    }

    public void ClickSpell0() {
        //controller.spell0;
    }
    public void ClickSpell1() {
        //controller.spell1;
    }
    public void ClickSpell2() {
        //controller.spell2;
    }
    public void ClickSpell3() {
        //controller.spell3;
    }
}

[System.Serializable]
public struct SpellInterface {
    public Button spellButton;
    public Image spellImage;
    public Text spellText;
}
