using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class PieceIdentificationScript : MonoBehaviour
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
	
	public MeshRenderer[] LightBulbs;
	public Material[] TheLights;
	
	public TextMesh[] Text;
	public TextMesh TextBox;
	public GameObject TheBox;
	
	bool Shifted = false;
	
	public AudioClip[] NotBuffer;
    public AudioClip[] PieceSections;
    private AudioClip PlayTheSection;
	private string pieceName = "";

	string[][] ChangedText = new string[2][]{
		new string[47] {"`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=", "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\", "a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'", "z", "x", "c", "v", "b", "n", "m", ",", ".", "/"},
		new string[47] {"~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "{", "}", "|", "A", "S", "D", "F", "G", "H", "J", "K", "L", ":", "\"", "Z", "X", "C", "V", "B", "N", "M", "<", ">", "?"}
	};
	
	bool Playable = false;
	bool Enterable = false;
	bool Toggleable = true;
	int Stages = 0;
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;
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
				Audio.PlaySoundAtTransform(NotBuffer[0].name, UselessButtons[Useless].transform);
				if (ThePieces.isPlaying && !Animating1)
				{
					ThePieces.Stop();
					ThePieces.clip = null;
				}
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
		Debug.LogFormat("[Piece Identification #{0}] Module inititated, and it's time to identifying classical pieces correctly. Good luck and show your best.", moduleId);
        int index = Random.Range(1, 5);
		ThePieces.clip = NotBuffer[index];
		ThePieces.Play();
        while (ThePieces.isPlaying)
		{
			yield return new WaitForSecondsRealtime(0.001f);
		}
		Playable = true;
		Startup = false;
	}
	
	void TypableKey(int KeyPress)
	{
		TypableText[KeyPress].AddInteractionPunch(.2f);
		Audio.PlaySoundAtTransform(NotBuffer[0].name, TypableText[KeyPress].transform);
		if (ThePieces.isPlaying && !StrikeIncoming)
        {
			ThePieces.Stop();
			ThePieces.clip = null;
        }
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
		Audio.PlaySoundAtTransform(NotBuffer[0].name, Backspace.transform);
		if (ThePieces.isPlaying && !Animating1)
		{
			ThePieces.Stop();
			ThePieces.clip = null;
		}
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
		Audio.PlaySoundAtTransform(NotBuffer[0].name, SpaceBar.transform);
		if (ThePieces.isPlaying && !Animating1)
		{
			ThePieces.Stop();
			ThePieces.clip = null;
		}
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
			StartCoroutine(PlayTheQueue());
		}
	}
	
	void PressEnter()
	{
		Enter.AddInteractionPunch(.2f);
		Audio.PlaySoundAtTransform(NotBuffer[0].name, Enter.transform);
		if (ThePieces.isPlaying && !Animating1)
		{
			ThePieces.Stop();
			ThePieces.clip = null;
		}
		if (Playable && Enterable)
		{
			StartCoroutine(TheCorrect());
		}
	}
	
	void PressShift(int Shifting)
	{
		ShiftButtons[Shifting].AddInteractionPunch(.2f);
		Audio.PlaySoundAtTransform(NotBuffer[0].name, ShiftButtons[Shifting].transform);
		if (ThePieces.isPlaying && !Animating1)
		{
			ThePieces.Stop();
			ThePieces.clip = null;
		}
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
	
	IEnumerator PlayTheQueue()
	{
		Toggleable = false;
		ActiveBorder = true;
		Playable = false;
        int index = Random.Range(0, 105);
        PlayTheSection = PieceSections[index];
		ThePieces.clip = PlayTheSection;
		if (index >= 0 && index < 25)
        {
			pieceName = "Sonata in C K.545, ";

		}
		else if (index >= 25 && index < 66)
		{
			pieceName = "Sonata in Eb Hob. XVI:52, ";

		}
		else if (index >= 67 && index < 105)
		{
			pieceName = "Sonata No.10 in G, Op.14, No.2, ";

		}
		if ((index >= 0 && index < 9) || (index >= 25 && index < 41) || (index >= 66 && index < 83))
        {
			pieceName = pieceName + "1st Movement";
        }
		else if ((index >= 9 && index < 17) || (index >= 41 && index < 49) || (index >= 83 && index < 93))
		{
			pieceName = pieceName + "2nd Movement";
        }
		else if ((index >= 17 && index < 25) || (index >= 49 && index < 66) || (index >= 93 && index < 105))
		{
			pieceName = pieceName + "3rd Movement";
        }
		ThePieces.Play();
		string sectionName = PlayTheSection.name;
		if (index == 16 || index == 24)
        {
			sectionName = "Part III - Coda";
        }
		else if (index == 60)
        {
			sectionName = "Recapitulation, First Subject";
        }
		else if (index == 103)
		{
			sectionName = "Coda";
		}
		else if (index == 49)
        {
			sectionName = "Exposition, First Subject";
        }
		Debug.LogFormat("[Piece Identification #{0}] The section played is from {1}. Specifically, it's {2}.", moduleId, pieceName, pieceName + ", " + sectionName);
		while (ThePieces.isPlaying)
        {
            yield return new WaitForSecondsRealtime(0.001f);
        }
		Playable = true;
		ActiveBorder = false;
		Enterable = true;
	}
	
	IEnumerator TheCorrect()
	{
		string Analysis = TextBox.text;
		TextBox.text = "";
		TextBox.characterSize = 0.001f;
		string sectionName = PlayTheSection.name;
		if (PlayTheSection.name == "Part III - Coda (2nd)" || PlayTheSection.name == "Part III - Coda (3rd)")
        {
			sectionName = "Part III - Coda";
        }
		else if (PlayTheSection.name == "Recapitulation, First Subject (3rd)")
		{
			sectionName = "Recapitulation, First Subject";
		}
		else if (PlayTheSection.name == "Coda (3rd)")
        {
			sectionName = "Coda";
        }
		else if (PlayTheSection.name == "Exposition, First Subject (3rd)")
		{
			sectionName = "Exposition, First Subject";
		}
		if (Analysis == pieceName || Analysis == pieceName + ", " + sectionName)
		{
			Playable = false;
			Enterable = false;
			if (Analysis == pieceName + ", " + sectionName)
            {
				Stages = 3;
            }
            else
            {
				Stages++;
			}
			if (Stages == 3)
			{
				Animating1 = true;
				Debug.LogFormat("[Piece Identification #{0}] You submitted {1}, which is either the full name of the segment, or you have successfully identified three segments correctly.", moduleId, Analysis);
                if (Bomb.GetTime() < 60)
                {
					int index = Random.Range(14, 17);
					ThePieces.clip = NotBuffer[index];
					ThePieces.Play();
                    LightBulbs[2].material = TheLights[1];
					LightBulbs[1].material = TheLights[1];
					LightBulbs[0].material = TheLights[1];
					ThePieces.clip = null;
				}
                else 
                {
					int index = Random.Range(14, 17);
					ThePieces.clip = NotBuffer[index];
					ThePieces.Play();
					while (ThePieces.isPlaying)
					{
						LightBulbs[2].material = TheLights[1];
						LightBulbs[1].material = TheLights[1];
						LightBulbs[0].material = TheLights[1];
						yield return new WaitForSecondsRealtime(0.075f);
						LightBulbs[2].material = TheLights[0];
						LightBulbs[1].material = TheLights[0];
						LightBulbs[0].material = TheLights[0];
						yield return new WaitForSecondsRealtime(0.075f);
					}
					ThePieces.clip = null;
				}
				LightBulbs[2].material = TheLights[1];
				LightBulbs[1].material = TheLights[1];
				LightBulbs[0].material = TheLights[1];
				Debug.LogFormat("[Piece Identification #{0}] The module will solve as a reward for you. Good job.", moduleId);
                Module.HandlePass();
				ModuleSolved = true;
                Animating1 = false;
			}
			
			else
			{
				Debug.LogFormat("[Piece Identification #{0}] You submitted {1}, which is correct.", moduleId, Analysis);
				Animating1 = true;
				int index = Random.Range(6, 10);
				ThePieces.clip = NotBuffer[index];
				ThePieces.Play();
                while (ThePieces.isPlaying)
                {
					LightBulbs[Stages - 1].material = TheLights[1];
					yield return new WaitForSecondsRealtime(0.075f);
                    LightBulbs[Stages - 1].material = TheLights[0];
		            yield return new WaitForSecondsRealtime(0.075f);
                }
				ThePieces.clip = null;
				LightBulbs[Stages - 1].material = TheLights[1];
				Playable = true;
				Toggleable = true;
				Animating1 = false;
			}
		}
		
		else
		{
			Debug.LogFormat("[Piece Identification #{0}] You submitted {1}, but that's wrong.", moduleId, Analysis);
			StrikeIncoming = true;
			Animating1 = true;
			int index = Random.Range(10, 14);
			ThePieces.clip = NotBuffer[index];
			ThePieces.Play();
			Enterable = false;
			while (ThePieces.isPlaying)
            {
				LightBulbs[0].material = TheLights[2];
				LightBulbs[1].material = TheLights[2];
				LightBulbs[2].material = TheLights[2];
				yield return new WaitForSecondsRealtime(0.3f);
				LightBulbs[0].material = TheLights[0];
				LightBulbs[1].material = TheLights[0];
				LightBulbs[2].material = TheLights[0];
				yield return new WaitForSecondsRealtime(0.3f);
			}
			ThePieces.clip = null;
            Debug.LogFormat("[Piece Identification #{0}] I'm very sorry, but I have to give a strike. Better luck next time!", moduleId);
			yield return new WaitForSecondsRealtime(0.5f);
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
			StrikeIncoming = false;
			Module.HandleStrike();
		}
	}
	
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To start the section, use !{0} play | To type in the text box, use !{0} type <text> | To submit, use !{0} submit | To clear the text, use !{0} [clear/fastclear] | Skip animations with !{0} skip (Strike and solve animations cannot be skipped)";
    #pragma warning restore 414
	
	int StartingNumber = 0;
	bool Startup = false;
	bool ActiveBorder = false;
	bool Animating1 = false;
	bool StrikeIncoming = false;
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
		
		else if (Regex.IsMatch(command, @"^\s*play\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
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

	IEnumerator TwitchHandleForcedSolve()
	{
		if (StrikeIncoming)
		{
			StopAllCoroutines();
			ThePieces.Stop();
			LightBulbs[0].material = TheLights[1];
			LightBulbs[1].material = TheLights[1];
			LightBulbs[2].material = TheLights[1];
			Module.HandlePass();
			yield break;
		}
		if (Stages != 3)
        {
			while (Animating1) yield return true;
			if (ThePieces.clip != null)
			{
				if (Startup)
				{
					Backspace.OnInteract();
					yield return new WaitForSecondsRealtime(0.05f);
					Border.OnInteract();
					yield return new WaitForSecondsRealtime(0.05f);
					Backspace.OnInteract();
					yield return new WaitForSecondsRealtime(0.05f);
				}
				else
				{
					Backspace.OnInteract();
					yield return new WaitForSecondsRealtime(0.05f);
				}
			}
			else if (Toggleable)
			{
				Border.OnInteract();
				yield return new WaitForSecondsRealtime(0.05f);
				Backspace.OnInteract();
				yield return new WaitForSecondsRealtime(0.05f);
			}
			string piece = pieceName;
			string sectionName = PlayTheSection.name;
			if (PlayTheSection.name == "Part III - Coda (2nd)" || PlayTheSection.name == "Part III - Coda (3rd)")
			{
				sectionName = "Part III - Coda";
			}
			else if (PlayTheSection.name == "Recapitulation, First Subject (3rd)")
			{
				sectionName = "Recapitulation, First Subject";
			}
			else if (PlayTheSection.name == "Coda (3rd)")
			{
				sectionName = "Coda";
			}
			else if (PlayTheSection.name == "Exposition, First Subject (3rd)")
			{
				sectionName = "Exposition, First Subject";
			}
			piece += ", " + sectionName;
			if (TextBox.text != piece)
			{
				int clearNum = -1;
				for (int j = 0; j < TextBox.text.Length; j++)
				{
					if (j == piece.Length)
						break;
					if (TextBox.text[j] != piece[j])
					{
						clearNum = j;
						int target = TextBox.text.Length - j;
						for (int k = 0; k < target; k++)
						{
							Backspace.OnInteract();
							yield return new WaitForSecondsRealtime(0.05f);
						}
						break;
					}
				}
				if (clearNum == -1)
				{
					if (TextBox.text.Length > piece.Length)
					{
						while (TextBox.text.Length > piece.Length)
						{
							Backspace.OnInteract();
							yield return new WaitForSecondsRealtime(0.05f);
						}
					}
					else
						yield return ProcessTwitchCommand("type " + piece.Substring(TextBox.text.Length));
				}
				else
					yield return ProcessTwitchCommand("type " + piece.Substring(clearNum));
			}
			Enter.OnInteract();
		}
		while (!ModuleSolved) { yield return true; }
	}
}