using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class UnownCipher : MonoBehaviour {
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    public KMSelectable[] Arrows;
    public KMSelectable[] LetterScreens;
    public KMSelectable SubmitButton;

    public TextMesh[] LetterTexts;
    public TextMesh[] StatScreens;
    public TextMesh SubmitText;
    public Color[] Colors;

    // Logging info
    private static int moduleIdCounter = 1;
    private int moduleId;
    private bool moduleSolved = false;
    private bool shinyFound = false;

    // Solving info
    private Unown[] unown = new Unown[5];

    // Information lists
    private string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
        "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" , "?"};

    private int[] letterIndexes = { 26, 26, 26, 26, 26 };

    // Calculation lists
    private int[] stat0List = { 0, 0, 64, 64, 128, 128, 192, 192, 0, 0, 64, 64, 128, 128, 192, 192 };
    private int[] stat1List = { 0, 0, 16, 16, 32, 32, 48, 48, 0, 0, 16, 16, 32, 32, 48, 48 };
    private int[] stat2List = { 0, 0, 4, 4, 8, 8, 12, 12, 0, 0, 4, 4, 8, 8, 12, 12 };
    private int[] stat3List = { 0, 0, 1, 1, 2, 2, 3, 3, 0, 0, 1, 1, 2, 2, 3, 3 };


    // Ran as bomb loads
    private void Awake() {
        moduleId = moduleIdCounter++;

        // Delegation
        SubmitButton.OnInteract += delegate () { SubmitButtonPress(); return false; };

        for (int i = 0; i < LetterScreens.Length; i++) {
            int j = i;
            LetterScreens[i].OnInteract += delegate () {
                LetterScreenPressed(j);
                return false;
            };
        }

        for (int i = 0; i < Arrows.Length; i++) {
            int j = i;
            Arrows[i].OnInteract += delegate () {
                ArrowPressed(j);
                return false;
            };
        }
    }

    // Starts the module
    private void Start() {
        // Sets up solving information
        CalculateUnown();

        // Congratulates the player for finding a shiny
        for (int i = 0; i < unown.Length; i++) {
            if (unown[i].getShiny() == true) {
                Debug.LogFormat("[Unown Cipher #{0}] Unown #{1} is shiny! Lucky you!", moduleId, i + 1);
                LetterTexts[i].color = Colors[2];

                // Plays only one sound if there are multiple shinies
                if (shinyFound == false) {
                    Audio.PlaySoundAtTransform("UnownCipher_Shiny", transform);
                    shinyFound = true;
                }
            }
        }

        for (int i = 0; i < LetterScreens.Length; i++)
            LetterTexts[i].text = "?";

        for (int i = 0; i < StatScreens.Length; i++)
            StatScreens[i].text = "--";
    }

    // Submit button is pressed
    private void SubmitButtonPress() {
        SubmitButton.AddInteractionPunch(0.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);

        if (moduleSolved == false) {
            bool seemsGood = true;

            // Tests if each letter is correct
            for (int i = 0; i < LetterScreens.Length && seemsGood == true; i++) {
                if (letterIndexes[i] != unown[i].getLetterValue())
                    seemsGood = false;
            }

            // Answer is correct
            if (seemsGood == true) {
                Debug.LogFormat("[Unown Cipher #{0}] Answer was correct! Module solved!", moduleId);
                moduleSolved = true;
                Audio.PlaySoundAtTransform("UnownCipher_Cry", transform);
                SubmitText.text = "solved";
                GetComponent<KMBombModule>().HandlePass();

                for (int i = 0; i < LetterScreens.Length; i++) {
                    LetterTexts[i].text = "!";

                    if (unown[i].getShiny() == true)
                        LetterTexts[i].color = Colors[2];

                    else
                        LetterTexts[i].color = Colors[0];
                }

                for (int i = 0; i < StatScreens.Length; i++)
                    StatScreens[i].text = "--";
            }

            else {
                Debug.LogFormat("[Unown Cipher #{0}] Answer was incorrect! Module struck!", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
            }
        }
    }

    // Arrowed keys are pressed
    private void ArrowPressed(int i) {
        Arrows[i].AddInteractionPunch(0.25f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);

        if (moduleSolved == false) {
            // Assigns arrow to screen
            int j = i % LetterScreens.Length;

            // Down arrow
            if (i >= LetterScreens.Length)
                letterIndexes[j]--;

            // Up arrow
            else
                letterIndexes[j]++;

            // Checks boundaries
            if (letterIndexes[j] > 26)
                letterIndexes[j] = 0;

            if (letterIndexes[j] < 0)
                letterIndexes[j] = 26;

            // Update screen text
            LetterTexts[j].text = letters[letterIndexes[j]];
        }
    }


    // Calculates stats
    private void CalculateUnown() {
        for (int i = 0; i < unown.Length; i++) {
            // Attributes
            int[] stats = { 0, 0, 0, 0 };
            string letter = "?";
            bool shiny = false;

            // Placeholders
            int letterValue = 0;


            // Creates stats
            for (int j = 0; j < stats.Length; j++)
                stats[j] = UnityEngine.Random.Range(0, 16);

            // Checks shiny rule
            if (stats[0] % 4 >= 2 && stats[1] == 10 && stats[2] == 10 && stats[3] == 10)
                shiny = true;

            // Gets letter
            letterValue = (int)Math.Floor(((double)(stat0List[stats[0]] + stat1List[stats[1]] + stat2List[stats[2]] + stat3List[stats[3]])) / 10.0);
            letter = letters[letterValue];

            unown[i] = new Unown(stats, letter, letterValue, shiny);

            Debug.LogFormat("[Unown Cipher #{0}] Unown #{1} has stats of {2}, {3}, {4}, and {5}. The correct letter is {6}.", moduleId, i + 1,
                stats[0], stats[1], stats[2], stats[3], letter);
        }     
    }


    // Displays stats
    private void LetterScreenPressed(int i) {
        LetterScreens[i].AddInteractionPunch(0.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, gameObject.transform);

        if (moduleSolved == false) {
            // Changes the colors of the letters to indicate which one is pressed
            for (int j = 0; j < LetterScreens.Length; j++) {
                if (unown[j].getShiny() == true)
                    LetterTexts[j].color = Colors[3];

                else
                    LetterTexts[j].color = Colors[1];
            }

            if (unown[i].getShiny() == true)
                LetterTexts[i].color = Colors[2];

            else
                LetterTexts[i].color = Colors[0];

            // Displays the stats
            if (unown[i].getStat0() < 10)
                StatScreens[0].text = "0" + unown[i].getStat0().ToString();

            else
                StatScreens[0].text = unown[i].getStat0().ToString();


            if (unown[i].getStat1() < 10)
                StatScreens[1].text = "0" + unown[i].getStat1().ToString();

            else
                StatScreens[1].text = unown[i].getStat1().ToString();


            if (unown[i].getStat2() < 10)
                StatScreens[2].text = "0" + unown[i].getStat2().ToString();

            else
                StatScreens[2].text = unown[i].getStat2().ToString();


            if (unown[i].getStat3() < 10)
                StatScreens[3].text = "0" + unown[i].getStat3().ToString();

            else
                StatScreens[3].text = unown[i].getStat3().ToString();
        }
    }


    // Twitch Plays - made by eXish
    private bool inputIsValid(string cmd)
    {
        string[] validstuff = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        for (int i = 0; i < cmd.Length; i++)
        {
            string temp = "";
            temp += cmd.ElementAt(i);
            temp = temp.ToUpper();
            if (!validstuff.Contains(temp))
            {
                return false;
            }
        }
        if (cmd.Length != 5)
        {
            return false;
        }
        return true;
    }

    private bool inputIsValid2(string cmd)
    {
        string[] validstuff = { "all", "1", "2", "3", "4", "5" };
        string temp = cmd.ToLower();
        if (validstuff.Contains(temp))
        {
            return true;
        }
        return false;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} submit ABCDE [Submits the 5 specified letters] | !{0} click <#>/all [Clicks the specified Unown (1-5 w/ 1 leftmost and 5 rightmost) or slowly clicks all]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*click\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 2)
            {
                if (inputIsValid2(parameters[1]))
                {
                    yield return null;
                    if (parameters[1].EqualsIgnoreCase("all"))
                    {
                        for(int i = 0; i < 5; i++)
                        {
                            LetterScreens[i].OnInteract();
                            yield return "trycancel Clicking cycle has been cancelled due to a cancel request.";
                            yield return new WaitForSeconds(2.5f);
                        }
                    }
                    else if (parameters[1].EqualsIgnoreCase("1"))
                    {
                        LetterScreens[0].OnInteract();
                    }
                    else if (parameters[1].EqualsIgnoreCase("2"))
                    {
                        LetterScreens[1].OnInteract();
                    }
                    else if (parameters[1].EqualsIgnoreCase("3"))
                    {
                        LetterScreens[2].OnInteract();
                    }
                    else if (parameters[1].EqualsIgnoreCase("4"))
                    {
                        LetterScreens[3].OnInteract();
                    }
                    else if (parameters[1].EqualsIgnoreCase("5"))
                    {
                        LetterScreens[4].OnInteract();
                    }
                }
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 2)
            {
                if (inputIsValid(parameters[1]))
                {
                    yield return null;
                    for(int i = 0; i < parameters[1].Length; i++)
                    {
                        string ch = "" + parameters[1].ElementAt(i);
                        ch = ch.ToUpper();
                        char ch2 = ch.ToCharArray()[0];
                        int index = char.ToUpper(ch2) - 64;
                        while(index != (letterIndexes[i]+1))
                        {
                            Arrows[i].OnInteract();
                            yield return new WaitForSeconds(0.025f);
                        }
                    }
                    SubmitButton.OnInteract();
                }
            }
            yield break;
        }
    }
}