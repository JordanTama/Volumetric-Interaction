using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using VolumetricInteraction.Benchmarking;
using Logger = VolumetricInteraction.Benchmarking.Logger;

public class RuntimeManager : MonoBehaviour
{
    public Controller controller;

    public GameObject home;
    public GameObject runtime;
    
    public Text elapsed;
    public Text id;
    public Text gpu;
    public Text cpu;
    public Text resolution;
    public Text sourceCount;
    public Text timeStep;
    public Text bruteForce;
    public Text decay;
    public Text fps;

    private float time;
    private Menu current;

    private Menu Current
    {
        get => current;
        set
        {
            current = value;
            UpdateMenu();
        }
    }

    private enum Menu
    {
        Home,
        Runtime
    }


    private void Start()
    {
        Current = Menu.Home;
    }

    private void Update()
    {
        if (!Current.Equals(Menu.Runtime) || !Logger.Active)
            return;
        
        elapsed.text = (Time.time - time).ToString(CultureInfo.CurrentCulture);
        id.text = Logger.DeviceUniqueIdentifier;
        gpu.text = Logger.GraphicsDeviceName;
        cpu.text = Logger.ProcessorType;
        resolution.text = Logger.Resolution;
        sourceCount.text = Logger.SourceCount;
        timeStep.text = Logger.TimeStep;
        bruteForce.text = Logger.UseBruteForce;
        decay.text = Logger.UseDecay;
        fps.text = Logger.FPS;
    }

    
    public void Begin()
    {
        Current = Menu.Runtime;
        controller.Benchmark();
        
        time = Time.time;
    }

    public void Return()
    {
        Current = Menu.Home;
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void UpdateMenu()
    {
        home.SetActive(current.Equals(Menu.Home));
        runtime.SetActive(current.Equals(Menu.Runtime));
    }
}
