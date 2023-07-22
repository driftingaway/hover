using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;
    public AudioSource a;
    public AudioClip clip;

    public TMP_InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect scrollRect;
    public GameObject msgList;
    public GameObject lastMsg;
    public List<GameObject> history;
    public List<string> inputHistory;
    public string userInput;
    public int histIndex = 0;

    public GameObject ChipSlot1;
    public GameObject ChipSlot2;

    public FMODUnity.EventReference TickEvent;
    public FMODUnity.EventReference KeyEvent;

    public GameObject shutter;
    public bool shutterStatus = true;
    OpenShutter openShutter;

    public UIManager ui;

    Interpreter interpreter;
    bool flip = true;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
        openShutter = shutter.GetComponent<OpenShutter>();
        List<GameObject> history = new List<GameObject>();
        terminalInput.onValueChanged.AddListener(delegate {ValueChangeCheck(KeyEvent); });
    }

    private void ValueChangeCheck(FMODUnity.EventReference Event)
    {
        FMODUnity.RuntimeManager.PlayOneShot(Event, transform.position);
    }

    private void OnGUI()
    {
        if(terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        { 
            // Store typed msg
            userInput = terminalInput.text;
            inputHistory.Add(userInput);
            histIndex = inputHistory.Count - 1;

            // Clear field
            ClearInputField();

            // Instantiate new game obj with a directory prefix
            AddDirectoryLine(userInput);

            // Add the interpreted lines
            AddInterpreterLines(interpreter.Interpret(userInput.ToLower()));
        }
    }

    private void Update()
    {
        // Up Arrow command to scroll through history
        if(terminalInput.isFocused && Input.GetKeyDown(KeyCode.UpArrow))
        {
            // update text
            terminalInput.text = inputHistory[histIndex];

            // move caret to end of msg
            terminalInput.caretPosition = terminalInput.text.Length;

            // update index
            histIndex -= 1;
            if(histIndex < 0) {
                histIndex = inputHistory.Count - 1;
            }
        }
    }

    private void ClearInputField()
    {
        terminalInput.text = "";
    }

    private void AddDirectoryLine(string userInput)
    {
        // Resize cmd container for scrollrect
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);

        // instantiate directory line
        GameObject msg = Instantiate(directoryLine, msgList.transform);
        history.Add(msg);

        // Set child index
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        // Set text of game obj
        msg.GetComponentsInChildren<TMP_Text>()[1].text = userInput;
    }

    void AddInterpreterLines(List<string> interpretation)
    {
        // coroutine for each line to wait until previous finishes
        StartCoroutine(StopAndWait());
        IEnumerator StopAndWait()
        {
            for(int i = 0; i < interpretation.Count; i++)
            {
                // Instantiate response line
                GameObject res = Instantiate(responseLine, msgList.transform);
                history.Add(res);

                // Place at end of msg list
                res.transform.SetAsLastSibling();

                // Get size of msg list and resize
                Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
                msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

                yield return StartCoroutine(fancyText(res, i));
            }

            // Enumerator to add a typewriter effect
            IEnumerator fancyText(GameObject res, int i)
            {
                // Scroll to the bottom of scrollrect
                ScrollToBottom(interpretation.Count);

                // Move user input line to the bottom
                userInputLine.transform.SetAsLastSibling();

                bool skip = false;
                string msg = "";
                int count = 0;

                // generate random interval speeds to type at
                List<float> randomVals = new List<float>{0.02f, 0.03f, 0.04f, 0.05f};
                int waitTime = Random.Range(0, 4);
                foreach (char c in interpretation[i])
                {
                    if(c == '>')
                    {
                        msg += c;
                        skip = false;
                        res.GetComponentInChildren<TMP_Text>().text = res.GetComponentInChildren<TMP_Text>().text + msg;
                        continue;
                    }

                    if(c == '<' || skip)
                    {
                        msg += c;
                        skip = true;
                        continue;
                    }

                    count += 1;
                    if(count > 4)
                    {
                        count = 0;
                        waitTime = Random.Range(0, 4);
                    }

                    // Set response line to returned interpreter string
                    yield return new WaitForSeconds(randomVals[waitTime]);

                    if(flip)
                    {
                        ValueChangeCheck(TickEvent);
                    }
                    flip = !flip;
                    res.GetComponentInChildren<TMP_Text>().text = res.GetComponentInChildren<TMP_Text>().text + c;
                }
            }
            // Refocus input field
            if(ui.isUIEnabled)
            {
                terminalInput.ActivateInputField();
                terminalInput.Select();
            }
        }
    }

    private void ScrollToBottom(int lines)
    {
        if(lines > 4)
        {
            scrollRect.velocity = new Vector2(0, 1000);
        }
        else
        {
            scrollRect.verticalNormalizedPosition = 0;
        }
    }

    public void ClearHistory()
    {
        foreach(GameObject msg in history)
        {
            Destroy(msg);
        }
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, 150.0f);
    }

    public List<string> ConnectedDevices()
    {
        List<string> ConnectedDevices = new List<string>(); 

        if(ChipSlot1.GetComponent<InsertChip>().orangeSlotted == true || ChipSlot2.GetComponent<InsertChip>().orangeSlotted == true)
        {
            ConnectedDevices.Add("Orange");
        }

        if(ChipSlot1.GetComponent<InsertChip>().blueSlotted == true || ChipSlot2.GetComponent<InsertChip>().blueSlotted == true)
        {
            ConnectedDevices.Add("Blue");
        }

        if(ChipSlot1.GetComponent<InsertChip>().purpleSlotted == true || ChipSlot2.GetComponent<InsertChip>().purpleSlotted == true)
        {
            ConnectedDevices.Add("Purple");
        }

        return ConnectedDevices;
    }

    public void ToggleShutter()
    {
        shutterStatus = !shutterStatus;
        if(shutterStatus)
        {
            openShutter.ToggleShutter(-1f);
        }
        else
        {
            openShutter.ToggleShutter(1f);
        }
    }
}
