using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static SpeechRecognizerPlugin;

public class SpeechRecognizer : MonoBehaviour, ISpeechRecognizerPlugin
{
    public static UnityEvent attack;
    public static UnityEvent shatterAttack;
    public static UnityEvent defend;
    private Queue<Action> actionQueue;
    private bool cr_running;

    private SpeechRecognizerPlugin plugin = null;
    void Start()
    {
        attack = new UnityEvent();
        shatterAttack = new UnityEvent();
        defend = new UnityEvent();
        actionQueue = new Queue<Action>();
        Invoke("Init", 3);
    }
    private void Init()
    {
        plugin = SpeechRecognizerPlugin.GetPlatformPluginVersion(this.gameObject.name);

        SetContinuousListening(true);

        SetSilenceLength(100);

        StartListening();
    }

    private void StartListening()
    {
        plugin.StartListening();
    }

    private void SetContinuousListening(bool isContinuous)
    {
        plugin.SetContinuousListening(isContinuous);
    }
    private void SetSilenceLength(int silenceLength)
    {
        plugin.SetSilenceLength(silenceLength);
    }

    public void OnResult(string recognizedResult)
    {
        char[] delimiterChars = { '~' };
        string[] result = recognizedResult.Split(delimiterChars);

        CheckForCommands(result);
    }
    private void CheckForCommands(string[] recognizedWords)
    {
        foreach (string sentence in recognizedWords)
        {
            Debug.Log(sentence);
        }

        foreach(string sentence in recognizedWords)
        {
            bool success = false;
            foreach (string word in sentence.Split(' '))
            {
                if (word.Equals("Fireball", StringComparison.InvariantCultureIgnoreCase))
                {
                    actionQueue.Enqueue(Attack);
                    success = true;
                }
                if (word.Equals("Shatter", StringComparison.InvariantCultureIgnoreCase))
                {
                    actionQueue.Enqueue(ShatterAttack);
                    success = true;
                }
                if (word.Equals("Shield", StringComparison.InvariantCultureIgnoreCase))
                {
                    actionQueue.Enqueue(Defend);
                    success = true;
                }
            }
            if(success)
            {
                if (!cr_running)
                {
                    StartCoroutine(ExecuteActions());
                }
                return;
            }
        }
    }
    private IEnumerator ExecuteActions()
    {
        cr_running = true;
        while (actionQueue.Count>0)
        {
            actionQueue.Dequeue().Invoke();
            yield return new WaitForSeconds(0.5f);
            cr_running = false;
        }
    }

    private void Attack()
    {
        attack.Invoke();
    }

    private void ShatterAttack()
    {
        shatterAttack.Invoke();
    }

    private void Defend()
    {
        defend.Invoke();
    }
}
