using System;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SlotState
{
    Empty,
    Full
}

public class SaveSlot : MonoBehaviour
{
    public SlotState slotState;

    public TMP_Text slotName;
    public TMP_Text slotDescription;
    public TMP_Text slotNumber;

    public string saveName;
    public int saveNumber;

    public GameObject emptyState;
    public GameObject fullState;

    public Button selectButton;
    public GameObject dialog;
    public Image picture;


    public void SetPicture(Texture2D picture)
    {
        Sprite sprite = Sprite.Create(picture, new Rect(0.0f, 0.0f, picture.width, picture.height), new Vector2(0.5f, 0.5f));
        this.picture.sprite = sprite;
    }

    public void ShowCoverDialog()
    {
        dialog.SetActive(true);
        dialog.GetComponent<SaveDialog>().text.text = "Are you sure to cover the save?";
        dialog.GetComponent<SaveDialog>().confirmButton.onClick.AddListener(SelectSlot);
    }

    public void ShowConfirmSaveDialog()
    {
        dialog.SetActive(true);
        dialog.GetComponent<SaveDialog>().text.text = "Are you sure to store the save?";
        dialog.GetComponent<SaveDialog>().confirmButton.onClick.AddListener(SelectSlot);
    }

    public void ShowConfirmLoadDialog()
    {
        dialog.SetActive(true);
        dialog.GetComponent<SaveDialog>().text.text = "Are you sure to load the save?";
        dialog.GetComponent<SaveDialog>().confirmButton.onClick.AddListener(SelectSlot);
    }

    public void SwitchToEmptyState()
    {
        fullState.SetActive(false);
        emptyState.SetActive(true);
        slotState = SlotState.Empty;
    }

    public void SwitchToFullState()
    {
        fullState.SetActive(true);
        emptyState.SetActive(false);
        slotState = SlotState.Full;
    }

    public virtual void OnEnable()
    {
        selectButton.onClick.AddListener(TrySelectSlot);
    }

    public virtual void OnDisable()
    {
        selectButton.onClick.RemoveAllListeners();
    }

    protected virtual void TrySelectSlot()
    {
        if (AVGSaveSystem.instance.state == StorageState.Save)
        {
            if (this.slotState == SlotState.Full)
            {
                if (ES3.FileExists(GetSlotPath(saveName)))
                {
                    ShowCoverDialog();
                    return;
                }
            }
            else
            {
                ShowConfirmSaveDialog();
                return;
            }
        }
        else
        {
            if (this.slotState == SlotState.Full)
            {
                if (ES3.FileExists(AVGSaveSystem.instance.GetSlotPath(saveName)))
                {
                    ShowConfirmLoadDialog();
                    return;
                }
            }
        }
    }

    protected virtual void SelectSlot()
    {
        dialog?.SetActive(false);
        if (AVGSaveSystem.instance.state == StorageState.Save)
        {
            if (ES3.FileExists(GetSlotPath(saveName)))
            {
                ES3.DeleteFile(GetSlotPath(saveName));
            }

            AVGSaveSystem.selectedSlotPath = GetSlotPath("save" + saveNumber.ToString());
            ES3Settings.defaultSettings.path = AVGSaveSystem.selectedSlotPath;
            ES3.Save("slotNumber", saveNumber);
            ES3.Save("slotPicture", ScreenCaptureSystem.CaptureCamera().EncodeToPNG());
            AVGSaveSystem.instance.onSave?.Invoke();
        }
        else
        {
            AVGSaveSystem.selectedSlotPath = GetSlotPath(saveName);
            ES3Settings.defaultSettings.path = AVGSaveSystem.selectedSlotPath;
            AVGSaveSystem.instance.onLoad?.Invoke();
        }
        AVGSaveSystem.instance.UpdateSaveSlots();
    }

    public virtual string GetSlotPath(string slotName)
    {
        return AVGSaveSystem.instance.GetSlotPath(slotName);
    }
}
