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
    public Text frameTime;
    public Text fps;
    public Text profileName;
    public Text test;

    private float _time;
    private Menu _current;

    private Menu Current
    {
        get => _current;
        set
        {
            _current = value;
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

        _time += Time.deltaTime;
        
        elapsed.text = _time.ToString(CultureInfo.CurrentCulture);
        id.text = Logger.DeviceUniqueIdentifier;
        gpu.text = Logger.GraphicsDeviceName;
        cpu.text = Logger.ProcessorType;
        resolution.text = Logger.Resolution;
        sourceCount.text = Logger.SourceCount;
        timeStep.text = Logger.TimeStep;
        bruteForce.text = Logger.UseBruteForce;
        decay.text = Logger.UseDecay;
        frameTime.text = Logger.FrameTime;
        fps.text = Logger.FPS;
        profileName.text = Logger.ProfileName;
        test.text = Logger.Test;
    }

    
    public void Begin()
    {
        Current = Menu.Runtime;
        controller.Benchmark();
        
        _time = 0;
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void UpdateMenu()
    {
        home.SetActive(_current.Equals(Menu.Home));
        runtime.SetActive(_current.Equals(Menu.Runtime));
    }
}
