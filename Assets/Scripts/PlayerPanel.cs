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

    private Color panelEnabledColor = new Color(0.75f, 0.75f, 0.75f, 1);
    private Color panelDisabledColor = new Color(0.5f, 0.5f, 0.5f, 1);

    public void SetMatchController(MatchController controller) {
        this.controller = controller;
    }
    public void SetPlayer(Player player) {
        this.player = player;
        playerNameText.text = player.name;
        panelEnabledColor = player.team == Team.TeamA ? new Color(0, .8f, 0, 1) : new Color(.8f, 0, .8f, 1);
        panelDisabledColor = player.team == Team.TeamA ? new Color(.2f, .4f, .2f, 1) : new Color(.4f, .3f, .4f, 1);
        for (int i = 0; i < 4; ++i) {
            spells[i].spellImage.sprite = Resources.Load<Sprite>(player.spells[i].spell.iconPath);
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
            if (player.spells[i].isRecharging) {
                spells[i].spellText.text = "" + player.spells[i].cooldown;
                spells[i].spellText.enabled = true;
            } else {
                spells[i].spellText.enabled = false;
            }
        }
    }

    public void SetPanelInteractable(bool value) {
        GetComponent<Image>().color = value ? panelEnabledColor : panelDisabledColor;
        for (int i = 0; i < 4; ++i) {
            spells[i].spellButton.interactable = value && !player.spells[i].isRecharging;
        }
    }

    public void ClickSpell0() {
        controller.OnClickSpell(0);
    }
    public void ClickSpell1() {
        controller.OnClickSpell(1);
    }
    public void ClickSpell2() {
        controller.OnClickSpell(2);
    }
    public void ClickSpell3() {
        controller.OnClickSpell(3);
    }
}

[System.Serializable]
public struct SpellInterface {
    public Button spellButton;
    public Image spellImage;
    public Text spellText;
}
