using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GamePlay : MonoBehaviour
{
    [SerializeField]
    public Text solution;
    private AudioSource source;

    [SerializeField]
    AudioClip wordTap, correctSFX, wrongSFX;

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
        words = wordsFile.text.Replace("\n", "").Replace("\r", "").Split(',');
        GenerateNewWord();
        Alphabet.pressed += AddAlphabet;
    }

    void AddAlphabet(char alphabet) {
        guess[index] = alphabet;
        index++;

        GetComponent<AudioSource>().Play();

        if (index >= currWord.Length) {
            StartCoroutine("VerifyWord");
        }
    }

    public void GenerateNewWord() {
        System.Random r = new System.Random();        
        var word = words[r.Next(words.Length - 1)];
        Debug.LogError(word);
        guess = new char[word.Length];
        currWord = word;
        solution.text = word;
        solution.enabled = false;

        index = 0;
        var random = new System.Random();

        char[] jumbled = (word.ToCharArray().OrderBy(x=> random.Next()).ToArray());
        foreach (var item in container)
        {
            Destroy( ((Transform) item).gameObject);
        }

        foreach (var item in jumbled)
        {
            GameObject go = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = container;
            go.GetComponent<Text>().text = item.ToString();
        }
    }

    IEnumerator VerifyWord() {
        if (currWord.Equals(new string(guess))) {
            solution.enabled = true;
            source.clip = correctSFX;
            yield return new WaitForSeconds(2f);
            GenerateNewWord();
            source.Play();
        } else {
            source.clip = wrongSFX;
            source.Play();
            yield return new WaitForSeconds(2f);
        }
        source.clip = wordTap;
    }
}