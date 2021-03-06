using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class VTuberIdentificationScript : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;
	public AudioSource TheMusic;
	
	public KMSelectable[] TypableText;
	public KMSelectable[] ShiftButtons;
	public KMSelectable[] UselessButtons;
	public KMSelectable Backspace;
	public KMSelectable Enter;
	public KMSelectable SpaceBar;
	public KMSelectable Border;
	
	public Material[] DesignBorder;
	public Material[] Background;
	public Material[] Logo;
	
	public MeshRenderer[] LightBulbs;
	public Material[] TheLights;
	public Material[] Vtubers;
	public Material QuestionMark;
	public Material Nice;
	public Material egg;
	
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
		int index = Random.Range(0, 2);
		ModelComponent.GetComponent<MeshRenderer>().material = DesignBorder[index];
		Surface.GetComponent<MeshRenderer>().material = Background[index];
		TheLogo.GetComponent<MeshRenderer>().material = Logo[index];
		Display.GetComponent<MeshRenderer>().material = QuestionMark;
		TheMusic.clip = SoundEffects[0];
		TheMusic.Play();
		yield return new WaitForSecondsRealtime(0.001f);
		Playable = true;
		Startup = false;
	}
	
	void TypableKey(int KeyPress)
	{
		TypableText[KeyPress].AddInteractionPunch(.2f);
		Audio.PlaySoundAtTransform(NotBuffer[0].name, TypableText[KeyPress].transform);
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
		Audio.PlaySoundAtTransform(NotBuffer[0].name, Enter.transform);
		if (Playable && Enterable)
		{
			StartCoroutine(TheCorrect());
		}
	}
	
	void PressShift(int Shifting)
	{
		ShiftButtons[Shifting].AddInteractionPunch(.2f);
		Audio.PlaySoundAtTransform(NotBuffer[0].name, ShiftButtons[Shifting].transform);
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
        int index = Random.Range(0, Vtubers.Length);
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
		if (Analysis == "Annoying Orange")
		{
			TheMusic.clip = SoundEffects[9];
			TheMusic.Play();
			Display.GetComponent<MeshRenderer>().material = egg;
		}
		else if (Analysis == VTuberName)
		{
			Stages++;
			Playable = false;
			Enterable = false;
			TheMusic.clip = SoundEffects[1];
			if (Stages == 3)
			{
				Animating1 = true;
				Debug.LogFormat("[VTuber Identification #{0}] Your guess is {1}, which is correct.", moduleId, Analysis);
                if (Bomb.GetTime() < 60)
                {
					TheMusic.Play();
                    LightBulbs[2].material = TheLights[1];
                }
                else 
                {
					TheMusic.Play();
					while (TheMusic.isPlaying)
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
				ModuleSolved = true;
				Animating1 = false;
			}
			
			else
			{
				Debug.LogFormat("[VTuber Identification #{0}] Your guess is {1}, which is correct! A stage pass for you.", moduleId, Analysis);
				TheMusic.Play();
				while (TheMusic.isPlaying)
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
			StrikeIncoming = true;
			Animating1 = true;
			Enterable = false;
			int index = Random.Range(2, 9);
			TheMusic.clip = SoundEffects[index];
			TheMusic.Play();
			while (TheMusic.isPlaying)
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
			StrikeIncoming = false;
			Module.HandleStrike();
		}
	}
	
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To reveal the VTuber, use !{0} start | To type in the text box, use !{0} type <text> | To submit, use !{0} submit | To clear the text, use !{0} [clear/fastclear]";
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
				yield return "sendtochaterror The module is still displaying the VTuber. Command was ignored";
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
			TheMusic.Stop();
			LightBulbs[0].material = TheLights[1];
			LightBulbs[1].material = TheLights[1];
			LightBulbs[2].material = TheLights[1];
			Display.GetComponent<MeshRenderer>().material = Nice;
			Module.HandlePass();
			yield break;
		}
		int start = Stages;
		for (int i = start; i < 3; i++)
		{
			while (!Playable) { yield return true; }
			if (Toggleable)
				Border.OnInteract();
			while (!Enterable) { yield return true; }
			if (TextBox.text != VTuberName)
			{
				int clearNum = -1;
				for (int j = 0; j < TextBox.text.Length; j++)
				{
					if (j == VTuberName.Length)
						break;
					if (TextBox.text[j] != VTuberName[j])
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
					if (TextBox.text.Length > VTuberName.Length)
					{
						while (TextBox.text.Length > VTuberName.Length)
						{
							Backspace.OnInteract();
							yield return new WaitForSecondsRealtime(0.05f);
						}
					}
					else
						yield return ProcessTwitchCommand("type " + VTuberName.Substring(TextBox.text.Length));
				}
				else
					yield return ProcessTwitchCommand("type " + VTuberName.Substring(clearNum));
			}
			Enter.OnInteract();
		}
		while (!ModuleSolved) { yield return true; }
	}
}