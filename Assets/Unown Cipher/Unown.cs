using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class Unown {
    // Attributes
    private int[] stats = { 0, 0, 0, 0 };
    private string letter = "?";
    private int letterValue = 26;
    private bool shiny = false;

    // Default setup
    public Unown(int[] stats, string letter, int letterValue, bool shiny) {
        this.stats = stats;
        this.letter = letter;
        this.letterValue = letterValue;
        this.shiny = shiny;
    }

    // Gets variables
    public int[] getStats() {
        return stats;
    }

    public int getStat0() {
        return stats[0];
    }

    public int getStat1() {
        return stats[1];
    }

    public int getStat2() {
        return stats[2];
    }

    public int getStat3() {
        return stats[3];
    }

    public string getLetter() {
        return letter;
    }

    public int getLetterValue() {
        return letterValue;
    }

    public bool getShiny() {
        return shiny;
    }


    // Sets variables
    public void setStats(int[] stats) {
        this.stats = stats;
    }

    public void setStat0(int stat) {
        stats[0] = stat;
    }

    public void setStat1(int stat) {
        stats[1] = stat;
    }

    public void setStat2(int stat) {
        stats[2] = stat;
    }

    public void setStat3(int stat) {
        stats[3] = stat;
    }

    public void setLetter(string letter) {
        this.letter = letter;
    }

    public void setLetterValue(int letterValue) {
        this.letterValue = letterValue;
    }

    public void setShiny(bool shiny) {
        this.shiny = shiny;
    }
}