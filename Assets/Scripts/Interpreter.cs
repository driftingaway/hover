using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    TerminalManager terminalManager;
    List<string> response = new List<string>();

    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"White", "#ffffff"},
        {"LightWhite", "#e0fbfc"},
        {"Grey", "#98c1d9"},
        {"BlueGrey", "#3d5a80"},
        {"Orange", "#ee6c4d"},
        {"DarkGrey", "#293241"}
    };

    List<string> entryList = new List<string>();
    List<string> connectedList = new List<string>();

    private void Start()
    {
        terminalManager = GetComponent<TerminalManager>();
        entryList.Add("procyon.txt");
        entryList.Add("testmsg.txt");
        entryList.Add("hbp.txt");
    }

    public List<string> Interpret(string userInput)
    {
        // clear messages
        response.Clear();

        string[] args = userInput.Split();

        if(args[0] == "help")
        {
            // return info
            ListEntry("help", "returns a list of commands", "Orange", "Grey");
            ListEntry("clear", "clears terminal history", "Orange", "Grey");
            ListEntry("list", "list available documents", "Orange", "Grey");
            ListEntry("read", "open file", "Orange", "Grey");
            ListEntry("usage", "read [filename]", "BlueGrey", "BlueGrey");
            ListEntry("ext", "show connected external drives", "Orange", "Grey");
            ListEntry("analyze", "analyze audio file", "Orange", "Grey");
            ListEntry("format", "fix corrupted files", "Orange", "Grey");
        }

        else if(args[0] == "ext")
        {
            List<string> devices = terminalManager.ConnectedDevices();

            response.Add("### CONNECTED DEVICES ###");
            if(devices.Count == 0)
            {
                response.Add("None");
            }
            else
            {
                foreach(string device in devices)
                {
                    response.Add(device);
                }
            }
        }

        else if(args[0] == "clear")
        {
            terminalManager.ClearHistory();
        }

        else if(args[0] == "list")
        {
            foreach(string i in entryList)
            {
                response.Add(i);
            }
        }

        else if(args[0] == "read") 
        {
            if((args.Length < 2) || (args[1] is not string)) 
            {
                response.Add("Invalid argument. Type " + ColorString("help", colors["Orange"]) + ColorString(" for more information.", colors["Grey"]));
            }
            
            else if(entryList.Contains(args[1]))
            {
                LoadTitle(args[1], "BlueGrey", 1);
            }

            else
            {
                response.Add("Unknown file. Type " + ColorString("list", colors["Orange"]) + ColorString(" for available documents.", colors["Grey"]));
            }
        }

        else
        {
            response.Add("Unknown command. Type " + ColorString("help", colors["Orange"]) + ColorString(" for a list of commands.", colors["Grey"]));
        }

        return response;

    }

    public string ColorString(string str, string color)
    {
        return "<color=" + color + ">" + str + "</color>";
    }

    void ListEntry(string a, string b, string color_a, string color_b)
    {
        response.Add(ColorString(a, colors[color_a]) + ": " + ColorString(b, colors[color_b]));
    }

    void LoadTitle(string path, string color, int spacing)
    {
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));
        AddSpace(spacing);
        while(!file.EndOfStream)
        {
            response.Add(ColorString(file.ReadLine(), colors[color]));
        }
        AddSpace(spacing);
        file.Close();
    }

    void AddSpace(int spacing)
    {
        for(int i = 0; i < spacing; i++)
        {
            response.Add("");
        }
    }

}
