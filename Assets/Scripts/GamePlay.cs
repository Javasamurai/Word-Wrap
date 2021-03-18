using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public class GamePlay : MonoBehaviour, IPointerDownHandler
{
    private bool started = false;
    private int score = 0;
    [SerializeField]
    public Text tapToPlayText, scoreText;
    public TextMeshProUGUI solution;
    private AudioSource source;

    [SerializeField]
    AudioClip wordTap = null, correctSFX = null, wrongSFX = null;

    [SerializeField]
    public GameObject characterPrefab;

    [SerializeField]
    public Transform container;

    string currWord = "";
    char[] guess;
    private int index = 0;
    List<Alphabet> alphabets;

    public TextAsset wordsFile;

    private string[] words;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = wordTap;
        scoreText.text = "Best:" + PlayerPrefs.GetInt("score") + "";
        words = wordsFile.text.Replace("\n", "").Replace("\r", "").Split(',');
        Alphabet.pressed += AddAlphabet;
    }

    void AddAlphabet(char alphabet) {
        guess[index] = alphabet;
        solution.text = new string(guess);
        index++;

        GetComponent<AudioSource>().Play();

        if (index >= currWord.Length) {
            StartCoroutine("VerifyWord");
        }
    }

    public void GenerateNewWord() {
        System.Random r = new System.Random();        
        var word = words[r.Next(words.Length - 1)];
        guess = new char[word.Length];
        currWord = word;
        solution.fontStyle = TMPro.FontStyles.Normal;
        solution.text = "";

        index = 0;
        char[] jumbled = ShuffleWord(word);
        foreach (var item in container)
        {
            Destroy( ((Transform) item).gameObject);
        }

        foreach (var item in jumbled)
        {
            GameObject go = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(container);
            go.transform.localScale = Vector3.one;
            go.GetComponent<Text>().text = item.ToString();
        }
    }

    char[] ShuffleWord(string word) {
        var random = new System.Random();

        char[] jumbled = word.ToCharArray().OrderBy(x=> random.Next()).ToArray(); 
        if (new string(jumbled).Equals(word)) {
            System.Random r = new System.Random();        
            var selectedWord = words[r.Next(words.Length - 1)];
            guess = new char[word.Length];
            currWord = word;
            return ShuffleWord(selectedWord);
        } else {
            return jumbled;
        }
    }

    IEnumerator VerifyWord() {
        solution.enabled = true;

        if (currWord.Equals(new string(guess))) {
            source.clip = correctSFX;
            score++;
            scoreText.text = score + "";
            yield return new WaitForSeconds(2f);
            solution.text = currWord;
            GenerateNewWord();
            source.Play();
        } else {
            if (score > PlayerPrefs.GetInt("score")) {
                PlayerPrefs.SetInt("score", score);
            }
            solution.fontStyle = TMPro.FontStyles.Strikethrough;
            score = 0;
            scoreText.text = score + "";
            source.clip = wrongSFX;
            source.Play();
            yield return new WaitForSeconds(2f);
            GenerateNewWord();
        }
        source.clip = wordTap;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
         if (!started) {
            started = true;
            tapToPlayText.gameObject.SetActive(false);
            scoreText.text = score + ""; 
            GenerateNewWord();
        }
    }
}