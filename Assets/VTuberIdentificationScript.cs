﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class VTuberIdentificationScript : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
	public AudioSource ThePieces;
	
	public KMSelectable[] TypableText;
	public KMSelectable[] ShiftButtons;
	public KMSelectable[] UselessButtons;
	public KMSelectable Backspace;
	public KMSelectable Enter;
	public KMSelectable SpaceBar;
	public KMSelectable Border;
	
	public SpriteRenderer MainSprite;
    public Sprite Beethoven;
    public Material[] ImageLighting;
	public Material[] DesignBorder;
	public Material[] Background;
	public Material[] Logo;
	
	public MeshRenderer[] LightBulbs;
	public Material[] TheLights;
	public Material[] Vtubers;
	public Material QuestionMark;
	public Material Nice;
	
	public TextMesh[] Text;
	public TextMesh TextBox;
	public GameObject TheBox;
	public GameObject Display;
	public GameObject ModelComponent;
	public GameObject Surface;
	public GameObject TheLogo;

	bool Shifted = false;
	private string VTuberName;
	
	public AudioClip[] NotBuffer;
	public AudioClip[] SoundEffects;
	
	string[][] ChangedText = new string[2][]{
		new string[47] {"`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=", "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\", "a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'", "z", "x", "c", "v", "b", "n", "m", ",", ".", "/"},
		new string[47] {"~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "{", "}", "|", "A", "S", "D", "F", "G", "H", "J", "K", "L", ":", "\"", "Z", "X", "C", "V", "B", "N", "M", "<", ">", "?"}
	};
	
	bool Playable = false;
	bool Enterable = false;
	bool Toggleable = true;
	int Stages = 0;
    int SolveSound = 0;
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;
        SolveSound = UnityEngine.Random.Range(0, 50);
		for (int b = 0; b < TypableText.Count(); b++)
        {
            int KeyPress = b;
            TypableText[KeyPress].OnInteract += delegate
            {
                TypableKey(KeyPress);
				return false;
            };
        }
		
		for (int a = 0; a < ShiftButtons.Count(); a++)
        {
            int Shifting = a;
            ShiftButtons[Shifting].OnInteract += delegate
            {
                PressShift(Shifting);
				return false;
            };
        }
		
		for (int c = 0; c < UselessButtons.Count(); c++)
        {
            int Useless = c;
            UselessButtons[Useless].OnInteract += delegate
            {
                UselessButtons[Useless].AddInteractionPunch(.2f);
				Audio.PlaySoundAtTransform(NotBuffer[0].name, transform);
				return false;
            };
        }
		
		Backspace.OnInteract += delegate () { PressBackspace(); return false; };
		Enter.OnInteract += delegate () { PressEnter(); return false; };
		SpaceBar.OnInteract += delegate () { PressSpaceBar(); return false; };
		Border.OnInteract += delegate () { PressBorder(); return false; };
	}
	
	
	void Start()
	{
		this.GetComponent<KMSelectable>().UpdateChildren();
		Module.OnActivate += Introduction;
	}
	
	void Introduction()
	{
		StartCoroutine(Reintroduction());
	}
	
	IEnumerator Reintroduction()
	{
		Startup = true;
		Debug.LogFormat("[VTuber Identification #{0}] The VTubers are ready to stream on Youtube! Better don't miss it now!", moduleId);
		int index = UnityEngine.Random.Range(0, 2);
		ModelComponent.GetComponent<MeshRenderer>().material = DesignBorder[index];
		Surface.GetComponent<MeshRenderer>().material = Background[index];
		TheLogo.GetComponent<MeshRenderer>().material = Logo[index];
		Display.GetComponent<MeshRenderer>().material = QuestionMark;
		ThePieces.clip = SoundEffects[0];
		ThePieces.Play();
		yield return new WaitForSecondsRealtime(0.001f);
		Playable = true;
		Startup = false;
	}
	
	void TypableKey(int KeyPress)
	{
		TypableText[KeyPress].AddInteractionPunch(.2f);
		ThePieces.clip = NotBuffer[0];
		ThePieces.Play();
		if (Playable && Enterable)
		{
			float width = 0;
			foreach (char symbol in TextBox.text)
			{
				CharacterInfo info;
				if (TextBox.font.GetCharacterInfo(symbol, out info, TextBox.fontSize, TextBox.fontStyle))
				{
					width += info.advance;
				}
			}
			width =  width * TextBox.characterSize * 0.1f;
			TextBox.text += Text[KeyPress].text;
			if (width > 0.28)
			{
				TextBox.characterSize -= 0.0001f;
			}
			
		}
	}
	
	void PressBackspace()
	{
		Backspace.AddInteractionPunch(.2f);
		ThePieces.clip = NotBuffer[0];
		ThePieces.Play();
        if (Playable)
		{
			if (TextBox.text.Length != 0)
			{
				string Copper = TextBox.text;
				Copper = Copper.Remove(Copper.Length - 1);
				TextBox.text = Copper;
			}
			float width = 0;
			foreach (char symbol in TextBox.text)
			{
				CharacterInfo info;
				if (TextBox.font.GetCharacterInfo(symbol, out info, TextBox.fontSize, TextBox.fontStyle))
				{
					width += info.advance;
				}
			}
			width = width * TextBox.characterSize * 0.1f;
			if (TextBox.characterSize < 0.001)
			{
				TextBox.characterSize += 0.0001f;
				if (width > 0.26)
                {
					TextBox.characterSize -= 0.0001f;
				}
			}
		}
	}
	
	void PressSpaceBar()
	{
		SpaceBar.AddInteractionPunch(.2f);
		ThePieces.clip = NotBuffer[0];
		ThePieces.Play();
        if (Playable && Enterable)
		{
			float width = 0;
			foreach (char symbol in TextBox.text)
			{
				CharacterInfo info;
				if (TextBox.font.GetCharacterInfo(symbol, out info, TextBox.fontSize, TextBox.fontStyle))
				{
					width += info.advance;
				}
			}
			width =  width * TextBox.characterSize * 0.1f;
			TextBox.text += " ";
			if (width > 0.28)
			{
				TextBox.characterSize -= 0.0001f;
			}
		}
	}
	
	void PressBorder()
	{
		Border.AddInteractionPunch(.2f);
		if (Playable && Toggleable)
		{
			StartCoroutine(VTuberReveal());
		}
	}
	
	void PressEnter()
	{
		Enter.AddInteractionPunch(.2f);
		ThePieces.clip = NotBuffer[0];
		ThePieces.Play();
        if (Playable && Enterable)
		{
			StartCoroutine(TheCorrect());
		}
	}
	
	void PressShift(int Shifting)
	{
		ShiftButtons[Shifting].AddInteractionPunch(.2f);
		ThePieces.clip = NotBuffer[0];
		ThePieces.Play();
        if (Shifted == true)
		{
			Shifted = false;
			StartingNumber = 0;
		}
		
		else
		{
			Shifted = true;
			StartingNumber = 1;
		}
		
		if (Shifted == true)
		{
			for (int b = 0; b < Text.Count(); b++)
			{
				Text[b].text = ChangedText[1][b];
			}
		}
		
		else
		{
			for (int a = 0; a < Text.Count(); a++)
			{
				Text[a].text = ChangedText[0][a];
			}
		}
	}
	
	IEnumerator VTuberReveal()
	{
		Toggleable = false;
		ActiveBorder = true;
		Playable = false;
        int index = UnityEngine.Random.Range(0, 174);
		Display.GetComponent<MeshRenderer>().material = Vtubers[index];
		VTuberName = Display.GetComponent<MeshRenderer>().material.name;
		VTuberName = VTuberName.Replace(" (Instance)", "");
		Debug.LogFormat("[VTuber Identification #{0}] In this stage {1} makes their debut!", moduleId, VTuberName);
        yield return new WaitForSecondsRealtime(5f);
		Display.GetComponent<MeshRenderer>().material = QuestionMark;
		Playable = true;
		ActiveBorder = false;
		Enterable = true;
	}
	
	IEnumerator TheCorrect()
	{
		string Analysis = TextBox.text;
		TextBox.text = "";
		if (Analysis == VTuberName)
		{
			Stages++;
			Playable = false;
			Enterable = false;
			switch (Analysis)
			{
				case "Akai Haato":
					ThePieces.clip = NotBuffer[1];
					break;
				case "Aki Rosenthal":
					ThePieces.clip = NotBuffer[2];
					break;
				case "Amane Kanata":
					ThePieces.clip = NotBuffer[3];
					break;
				case "Amano Pikamee":
					ThePieces.clip = NotBuffer[22];
					break;
				case "AZKi":
					ThePieces.clip = NotBuffer[11];
					break;
				case "Dennou Shoujo Siro":
					ThePieces.clip = NotBuffer[4];
					break;
				case "Gawr Gura":
					ThePieces.clip = NotBuffer[5];
					break;
				case "Himemori Luna":
					ThePieces.clip = NotBuffer[6];
					break;
				case "Hikasa Tomoshika":
					ThePieces.clip = NotBuffer[22];
					break;
				case "Hoshimachi Suisei":
					ThePieces.clip = NotBuffer[7];
					break;
				case "Houshou Marine":
					ThePieces.clip = NotBuffer[8];
					break;
				case "Inugami Korone":
					ThePieces.clip = NotBuffer[9];
					break;
				case "Kamiko Kana":
					ThePieces.clip = NotBuffer[47];
					break;
				case "Kaguya Luna":
					ThePieces.clip = NotBuffer[10];
					break;
				case "Kizuna AI":
					ThePieces.clip = NotBuffer[12];
					break;
				case "Natsuiro Matsuri":
					ThePieces.clip = NotBuffer[13];
					break;
				case "Minato Aqua":
					ThePieces.clip = NotBuffer[14];
					break;
				case "Momosuzu Nene":
					ThePieces.clip = NotBuffer[15];
					break;
				case "Mori Calliope":
					ThePieces.clip = NotBuffer[40];
					break;
				case "Murasaki Shion":
					ThePieces.clip = NotBuffer[16];
					break;
				case "Nakiri Ayame":
					ThePieces.clip = NotBuffer[17];
					break;
				case "Nekomata Okayu":
					ThePieces.clip = NotBuffer[18];
					break;
				case "Ninomae Ina'nis":
					ThePieces.clip = NotBuffer[41];
					break;
				case "Omaru Polka":
					ThePieces.clip = NotBuffer[19];
					break;
				case "Ookami Mio":
					ThePieces.clip = NotBuffer[20];
					break;
				case "Oozora Subaru":
					ThePieces.clip = NotBuffer[21];
					break;
				case "Roboco":
					ThePieces.clip = NotBuffer[23];
					break;
				case "Sakura Miko":
					ThePieces.clip = NotBuffer[24];
					break;
				case "Shirakami Fubuki":
					ThePieces.clip = NotBuffer[25];
					break;
				case "Shiranui Flare":
					ThePieces.clip = NotBuffer[26];
					break;
				case "Shirogane Noel":
					ThePieces.clip = NotBuffer[27];
					break;
				case "Shishiro Botan":
					ThePieces.clip = NotBuffer[28];
					break;
				case "Suzuki Hina":
					ThePieces.clip = NotBuffer[39];
					break;
				case "Takanashi Kiara":
					ThePieces.clip = NotBuffer[46];
					break;
				case "Tanaka Hime":
					ThePieces.clip = NotBuffer[39];
					break;
				case "Tokino Sora":
					ThePieces.clip = NotBuffer[30];
					break;
				case "Tokoyami Towa":
					ThePieces.clip = NotBuffer[31];
					break;
				case "Tsunomaki Watame":
					ThePieces.clip = NotBuffer[32];
					break;
				case "Uruha Rushia":
					ThePieces.clip = NotBuffer[33];
					break;
				case "Usada Pekora":
					ThePieces.clip = NotBuffer[34];
					break;
				case "Virtual Noja Loli Kitsunemusume YouTuber Ojisan":
					ThePieces.clip = NotBuffer[29];
					break;
				case "Watson Amelia":
					ThePieces.clip = NotBuffer[35];
					break;
				case "Yozora Mel":
					ThePieces.clip = NotBuffer[36];
					break;
				case "Yukihana Lamy":
					ThePieces.clip = NotBuffer[37];
					break;
				case "Yuzuki Choco":
					ThePieces.clip = NotBuffer[38];
					break;
				default:
					ThePieces.clip = SoundEffects[1];
					break;
			}
			if (Stages == 3)
			{
				Animating1 = true;
				Debug.LogFormat("[VTuber Identification #{0}] Your guess is {1}, which is correct.", moduleId, Analysis);
                if (Bomb.GetTime() < 60)
                {
					ThePieces.Play();
                    LightBulbs[2].material = TheLights[1];
                }
                else 
                {
					ThePieces.Play();
					while (ThePieces.isPlaying)
					{
						LightBulbs[Stages - 1].material = TheLights[1];
						yield return new WaitForSecondsRealtime(0.075f);
						LightBulbs[Stages - 1].material = TheLights[0];
						yield return new WaitForSecondsRealtime(0.075f);
					}
				}
				Debug.LogFormat("[VTuber Identification #{0}] You have been proven to be their most loyal fan. Module solved sucessfully.", moduleId);
				Display.GetComponent<MeshRenderer>().material = Nice;
				LightBulbs[2].material = TheLights[1];		
				Module.HandlePass();
				Animating1 = false;
			}
			
			else
			{
				Debug.LogFormat("[VTuber Identification #{0}] Your guess is {1}, which is correct! A stage pass for you.", moduleId, Analysis);
				ThePieces.Play();
				while (ThePieces.isPlaying)
				{
					LightBulbs[Stages - 1].material = TheLights[1];
					yield return new WaitForSecondsRealtime(0.075f);
					LightBulbs[Stages - 1].material = TheLights[0];
					yield return new WaitForSecondsRealtime(0.075f);
				}
                    LightBulbs[Stages - 1].material = TheLights[1];
        
				Playable = true;
				Toggleable = true;
				Animating1 = false;
			}
		}
		
		else
		{
			Debug.LogFormat("[VTuber Identification #{0}] Your guess is {1}, but that's not their name, strike.", moduleId, Analysis);
			Animating1 = true;
			Enterable = false;
			int index = UnityEngine.Random.Range(2, 9);
			ThePieces.clip = SoundEffects[index];
			ThePieces.Play();
			while (ThePieces.isPlaying)
            {
				LightBulbs[0].material = TheLights[2];
				LightBulbs[1].material = TheLights[2];
				LightBulbs[2].material = TheLights[2];
				yield return new WaitForSecondsRealtime(1f);
				LightBulbs[0].material = TheLights[0];
				LightBulbs[1].material = TheLights[0];
				LightBulbs[2].material = TheLights[0];
				yield return new WaitForSecondsRealtime(1f);
			}
			yield return new WaitForSecondsRealtime(1f);
			LightBulbs[0].material = TheLights[0];
			LightBulbs[1].material = TheLights[0];
			LightBulbs[2].material = TheLights[0];
			if (Stages == 0)
            {
				LightBulbs[0].material = TheLights[0];
				LightBulbs[1].material = TheLights[0];
				LightBulbs[2].material = TheLights[0];
			}
			else if (Stages == 1)
            {
				LightBulbs[0].material = TheLights[1];
				LightBulbs[1].material = TheLights[0];
				LightBulbs[2].material = TheLights[0];
			}
			else if (Stages == 2)
            {
				LightBulbs[0].material = TheLights[1];
				LightBulbs[1].material = TheLights[1];
				LightBulbs[2].material = TheLights[0];
			}
			Playable = true;
			Toggleable = true;
			Animating1 = false;
			Module.HandleStrike();
		}
	}
	
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To reveal the VTuber, use !{0} start | To type in the text box, use !{0} type <text> | To submit, use !{0} submit | To clear the text, use !{0} [clear/fastclear] | Skip animations with !{0} skip (Strike animations cannot be skipped)";
    #pragma warning restore 414
	
	int StartingNumber = 0;
	bool Startup = false;
	bool ActiveBorder = false;
	bool Animating1 = false;
	string Current = "";
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(parameters[0], @"^\s*type\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			
			if (Startup == true)
			{
				yield return "sendtochaterror The introduction music is still playing. Command was ignored";
				yield break;
			}
			
			if (ActiveBorder == true)
			{
				yield return "sendtochaterror The module is still displaying the VTuber. Command was ignored";
				yield break;
			}
			
			if (Animating1 == true)
			{
				yield return "sendtochaterror The module is performing an animation. Command was ignored";
				yield break;
			}
			
			if (Enterable == false)
			{
				yield return "sendtochaterror The keys are not yet pressable. Command was ignored";
				yield break;
			}
			
			for (int x = 0; x < parameters.Length - 1; x++)
			{
				foreach (char c in parameters[x+1])
				{
					if (!c.ToString().EqualsAny(ChangedText[0]) && !c.ToString().EqualsAny(ChangedText[1]))
					{
						yield return "sendtochaterror The command being submitted contains a character that is not + typable in the given keyboard";
						yield break;
					}
				}
			}
			
			for (int y = 0; y < parameters.Length - 1; y++)
			{
				yield return "trycancel The command to type the text given was halted due to a cancel request";
				foreach (char c in parameters[y+1])
				{
					yield return "trycancel The command to type the text given was halted due to a cancel request";
					Current = TextBox.text;
					if (!c.ToString().EqualsAny(ChangedText[StartingNumber]))
					{
						ShiftButtons[0].OnInteract();
						yield return new WaitForSecondsRealtime(0.05f);
					}
					
					for (int z = 0; z < ChangedText[StartingNumber].Count(); z++)
					{
						if (c.ToString() == ChangedText[StartingNumber][z])
						{
							TypableText[z].OnInteract();
							yield return new WaitForSecondsRealtime(0.05f);
							break;
						}
					}
					
					if (Current == TextBox.text)
					{
						yield return "sendtochaterror The command was stopped due to the text box not able to recieve more characters";
						yield break;
					}
				}

				if (y != parameters.Length - 2)
				{
					SpaceBar.OnInteract();
					yield return new WaitForSecondsRealtime(0.05f);
				}
				
				if (Current == TextBox.text)
				{
					yield return "sendtochaterror The command was stopped due to the text box not able to recieve more characters";
					yield break;
				}
			}
		}
		
		else if (Regex.IsMatch(command, @"^\s*clear\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			
			if (Startup == true)
			{
				yield return "sendtochaterror The introduction music is still playing. Command was ignored";
				yield break;
			}
			
			if (ActiveBorder == true)
			{
				yield return "sendtochaterror The module is still playing the section. Command was ignored";
				yield break;
			}
			
			if (Animating1 == true)
			{
				yield return "sendtochaterror The module is performing an animation. Command was ignored";
				yield break;
			}
			
			if (Enterable == false)
			{
				yield return "sendtochaterror The key is not yet pressable. Command was ignored";
				yield break;
			}
			
			while (TextBox.text.Length != 0)
			{
				yield return "trycancel The command to clear text in the text box was halted due to a cancel request";
				Backspace.OnInteract();
				yield return new WaitForSecondsRealtime(0.05f);
			}
		}
		
		else if (Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			
			if (Startup == true)
			{
				yield return "sendtochaterror The introduction music is still playing. Command was ignored";
				yield break;
			}
			
			if (ActiveBorder == true)
			{
				yield return "sendtochaterror The module is still playing the section. Command was ignored";
				yield break;
			}
			
			if (Animating1 == true)
			{
				yield return "sendtochaterror The module is performing an animation. Command was ignored";
				yield break;
			}
			
			if (Enterable == false)
			{
				yield return "sendtochaterror The key is not yet pressable. Command was ignored";
				yield break;
			}
			yield return "solve";
			yield return "strike";
				Enter.OnInteract();
		}
		
		else if (Regex.IsMatch(command, @"^\s*start\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			
			if (Startup == true)
			{
				yield return "sendtochaterror The introduction music is still playing. Command was ignored.";
				yield break;
			}
			
			if (ActiveBorder == true)
			{
				yield return "sendtochaterror The module is still playing the section. Command was ignored";
				yield break;
			}
			
			if (Animating1 == true)
			{
				yield return "sendtochaterror The module is performing an animation. Command was ignored";
				yield break;
			}
			
			if (Enterable == true)
			{
				yield return "sendtochaterror You are not able to press the border again. Command was ignored";
				yield break;
			}
			
			Border.OnInteract();
		}

        else if (Regex.IsMatch(command, @"^\s*skip\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

            if (Startup == true)
            {
                Backspace.OnInteract();
            }
            else if (ActiveBorder == true)
            {
                Backspace.OnInteract();
            }
            else if (Animating1 == true)
            {
                Backspace.OnInteract();
            }
            else
            {
                yield return "sendtochaterror Nothing's skippable currently. Command was ignored.";
                yield break;
            }
        }

        else if (Regex.IsMatch(command, @"^\s*fastclear\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			
			if (Startup == true)
			{
				yield return "sendtochaterror The introduction music is still playing. Command was ignored.";
				yield break;
			}
			
			if (ActiveBorder == true)
			{
				yield return "sendtochaterror The module is still playing the section. Command was ignored";
				yield break;
			}
			
			if (Animating1 == true)
			{
				yield return "sendtochaterror The module is performing an animation. Command was ignored";
				yield break;
			}
			
			if (Enterable == false)
			{
				yield return "sendtochaterror The key is not yet pressable. Command was ignored";
				yield break;
			}
			
			while (TextBox.text.Length != 0)
			{
				Backspace.OnInteract();
			}
		}
	}
}
