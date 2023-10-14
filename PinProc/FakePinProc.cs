using NetPinProc.Domain.MachineConfig;
using NetPinProc.Domain.Pdb;
using NetPinProc.Domain.PinProc;
using System;
using System.Collections.Generic;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Fake P-ROC device, no calls to PinProc. Is a <see cref="IProcDevice"/>
    /// </summary>
    public class FakePinProc : IFakeProcDevice
    {
        private MachineType _machineType;        
        private int frames_per_second = 60;
        private double last_dmd_event = 0;
        private double now;
        private List<Event> switch_events = new List<Event>();
        private FakeSwitchRule[] switch_rules = new FakeSwitchRule[1024];

        /// <summary>
        /// <inheritdoc/> <para/>
        /// Initialized with 256 drivers
        /// </summary>
        public AttrCollection<ushort, string, IVirtualDriver> Drivers { get; private set; }

        /// <summary>
        /// Creates Virtual drivers, the drivers are accessed from methods in this class. Creates 1,024 fake switch_rules
        /// </summary>
        /// <param name="machineType"></param>
        /// <param name="logger"></param>
        public FakePinProc(MachineType machineType, ILogger logger)
        {
            _machineType = machineType;
            Logger = logger;

            //Add the virtual drivers.
            this.Drivers = new AttrCollection<ushort, string, IVirtualDriver>();
            for (ushort i = 0; i < 256; i++)
            {
                string name = "driver" + i.ToString();
                Drivers.Add(i, name, new VirtualDriver(this, name, i, true));
            }

            // Instantiate default switch rules
            for (int j = 0; j < 1024; j++)
            {
                switch_rules[j] = new FakeSwitchRule() { NotifyHost = true, ReloadActive = false, Drivers = new List<IDriver>() };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ILogger Logger { get; set; }
        /// <summary>
        /// Adds switch event to <see cref="switch_events"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="event_type"></param>
        public void AddSwitchEvent(ushort number, EventType event_type)
        {
            int rule_index = (((int)event_type - 1) * 256) + number;

            if (this.switch_rules[rule_index].NotifyHost)
            {
                Event evt = new Event() { Type = event_type, Value = number };
                this.switch_events.Add(evt);
            }

            List<IDriver> dlist = this.switch_rules[rule_index].Drivers;
            foreach (IDriver drv in dlist)
                this.Drivers[drv.Number].State = drv.State;
        }
        /// <summary>
        /// nothing
        /// </summary>
        /// <param name="address"></param>
        /// <param name="aux_commands"></param>
        public void AuxSendCommands(ushort address, ushort aux_commands) { }
        /// <summary>
        /// nothing
        /// </summary>
        public void Close() { }
        /// <summary>
        /// nothing
        /// </summary>
        /// <param name="bytes"></param>
        public void DmdDraw(byte[] bytes) { }
        /// <summary>
        /// nothing
        /// </summary>
        /// <param name="frame"></param>
        public void DmdDraw(IFrame frame) { }
        /// <summary>
        /// nothing
        /// </summary>
        /// <param name="high_cycles"></param>
        public void DmdUpdateConfig(ushort[] high_cycles) { }
        /// <summary>
        /// Disables driver from <see cref="Drivers"/>
        /// </summary>
        /// <param name="number"></param>
        public void DriverDisable(ushort number) => this.Drivers[number].Disable();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="milliseconds"></param>
        /// <param name="futureTime"></param>
        /// <returns><see cref="Result.Success"/></returns>
        public Result DriverFuturePulse(ushort number, byte milliseconds, UInt16 futureTime) => Result.Success;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public DriverState DriverGetState(ushort number) => this.Drivers[number].State;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public void DriverGroupDisable(byte number) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="milliseconds_on"></param>
        /// <param name="milliseconds_off"></param>
        /// <param name="original_on_time"></param>
        public void DriverPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public Result DriverPulse(ushort number, byte milliseconds)
        {
            this.Drivers[number].Pulse(milliseconds);
            return Result.Success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="milliseconds_on"></param>
        /// <param name="milliseconds_off"></param>
        /// <param name="milliseconds_overall_patter_time"></param>
        public void DriverPulsedPatter(ushort number, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="schedule"></param>
        /// <param name="cycle_seconds"></param>
        /// <param name="now"></param>
        public void DriverSchedule(ushort number, uint schedule, ushort cycle_seconds, bool now)
            => this.Drivers[number].Schedule(schedule, cycle_seconds, now);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public DriverState DriverStateDisable(DriverState state) => state;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="milliseconds"></param>
        /// <param name="futureTime"></param>
        /// <returns></returns>
        public DriverState DriverStateFuturePulse(DriverState state, byte milliseconds, ushort futureTime) => state;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="milliseconds_on"></param>
        /// <param name="milliseconds_off"></param>
        /// <param name="original_on_time"></param>
        /// <returns></returns>
        public DriverState DriverStatePatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort original_on_time) => state;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public DriverState DriverStatePulse(DriverState state, byte milliseconds) => state;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="milliseconds_on"></param>
        /// <param name="milliseconds_off"></param>
        /// <param name="milliseconds_overall_patter_time"></param>
        /// <returns></returns>
        public DriverState DriverStatePulsedPatter(DriverState state, ushort milliseconds_on, ushort milliseconds_off, ushort milliseconds_overall_patter_time) => state;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="schedule"></param>
        /// <param name="seconds"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public DriverState DriverStateSchedule(DriverState state, uint schedule, byte seconds, bool now) => state;
        /// <summary>
        /// nothing
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="polarity"></param>
        /// <param name="use_clear"></param>
        /// <param name="strobe_start_select"></param>
        /// <param name="start_strobe_time"></param>
        /// <param name="matrix_row_enable_index0"></param>
        /// <param name="matrix_row_enable_index1"></param>
        /// <param name="active_low_matrix_rows"></param>
        /// <param name="tickle_stern_watchdog"></param>
        /// <param name="encode_enables"></param>
        /// <param name="watchdog_expired"></param>
        /// <param name="watchdog_enable"></param>
        /// <param name="watchdog_reset_time"></param>
        public void DriverUpdateGlobalConfig(bool enable, bool polarity, bool use_clear, bool strobe_start_select, byte start_strobe_time, byte matrix_row_enable_index0, byte matrix_row_enable_index1, bool active_low_matrix_rows, bool tickle_stern_watchdog, bool encode_enables, bool watchdog_expired, bool watchdog_enable, ushort watchdog_reset_time) { }
        /// <summary>
        /// nothing
        /// </summary>
        /// <param name="group_num"></param>
        /// <param name="slow_time"></param>
        /// <param name="enable_index"></param>
        /// <param name="row_activate_index"></param>
        /// <param name="row_enable_select"></param>
        /// <param name="matrixed"></param>
        /// <param name="polarity"></param>
        /// <param name="active"></param>
        /// <param name="disable_strobe_after"></param>
        public void DriverUpdateGroupConfig(byte group_num, ushort slow_time, byte enable_index, byte row_activate_index, byte row_enable_select, bool matrixed, bool polarity, bool active, bool disable_strobe_after) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public void DriverUpdateState(ref DriverState driver) => this.Drivers[driver.DriverNum].State = driver;
        /// <summary>
        /// Does nothing
        /// </summary>
        public void Flush() { }
        /// <summary>
        /// Gets events from local <see cref="switch_events"/>
        /// </summary>
        /// <param name="dmdEvents"></param>
        /// <returns></returns>
        public Event[] Getevents(bool dmdEvents = true)
        {
            List<Event> events = new List<Event>();
            events.AddRange(this.switch_events);
            this.switch_events.Clear();

            if (dmdEvents)
            {
                now = Time.GetTime();
                double seconds_since_last_dmd_event = now - this.last_dmd_event;
                int missed_dmd_events = Math.Min((int)seconds_since_last_dmd_event * this.frames_per_second, 16);
                if (missed_dmd_events > 0)
                {
                    this.last_dmd_event = now;
                    for (int i = 0; i < missed_dmd_events; i++)
                        events.Add(new Event() { Type = EventType.DMDFrameDisplayed, Value = 0 });
                }
            }            
            return events.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="register"></param>
        /// <param name="value"></param>
        public void i2c_write8(uint address, uint register, uint value) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
		public void initialize_i2c(uint address) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="startingAddr"></param>
        /// <param name="data"></param>
        /// <returns><see cref="Result.Success"/></returns>
        public Result ReadData(uint module, uint startingAddr, ref uint data) => Result.Success;
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="flags"></param>
        public void Reset(uint flags) { }
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="mapping"></param>
        public void SetDmdColorMapping(byte[] mapping) { }
        /// <summary>
        /// This can be used fake but the project should be x86 to be in-line with the library. <para/>
        /// Only method to use the PinProc.PRDecode. TODO: This method could be added from the PRDecode and this class moved away from this project, so that it's not dependent on x86
        /// </summary>
        /// <param name="config"></param>
        /// <param name="_coils">PDB coil number 0 = P-ROC number 40</param>
        /// <param name="_switches"></param>
        /// <param name="_lamps"></param>
        /// <param name="_leds"></param>
        /// <param name="_gi"></param>
        /// <param name="_steppers"></param>
        /// <param name="_servos"></param>
        /// <param name="_pdSerialLeds"></param>
        public void SetupProcMachine(MachineConfiguration config,
            AttrCollection<ushort, string, IDriver> _coils = null,
            AttrCollection<ushort, string, Switch> _switches = null,
            AttrCollection<ushort, string, IDriver> _lamps = null,
            AttrCollection<ushort, string, LED> _leds = null,
            AttrCollection<ushort, string, IDriver> _gi = null,
            AttrCollection<ushort, string, PdStepper> _steppers = null,
            AttrCollection<ushort, string, PdServo> _servos = null,
            AttrCollection<ushort, string, PdSerialLed> _pdSerialLeds = null)
        {
            List<VirtualDriver> new_virtual_drivers = new List<VirtualDriver>();
            bool polarity = (_machineType == MachineType.SternWhitestar || _machineType == MachineType.SternSAM || _machineType == MachineType.PDB);

            PDBConfig pdb_config = null;
            if (_machineType == MachineType.PDB)
                pdb_config = new PDBConfig(this, config);

            //process and add coils, add virtual driver, drivers
            if (_coils != null)
            {
                foreach (CoilConfigFileEntry ce in config.PRCoils)
                {
                    Driver d;
                    int number;
                    if (_machineType == MachineType.PDB && pdb_config != null)
                    {
                        number = pdb_config.GetProcNumber("PRCoils", ce.Number);

                        if (number == -1)
                        {
                            Console.WriteLine("Coil {0} cannot be controlled by the P-ROC. Ignoring...", ce.Name);
                            continue;
                        }
                    }
                    else
                        number = PinProcDecoder.PRDecode(_machineType, ce.Number);

                    if ((ce.Bus != null && ce.Bus == "AuxPort") || number >= PinProcConstants.kPRDriverCount)
                    {
                        d = new VirtualDriver(this, ce.Name, (ushort)number, polarity);
                        new_virtual_drivers.Add((VirtualDriver)d);
                    }
                    else
                    {
                        d = new Driver(this, ce.Name, (ushort)number);
                        Logger?.Log("Adding driver " + d.ToString(), LogLevel.Verbose);
                        d.ReConfigure(ce.Polarity);
                    }
                    _coils.Add(d.Number, d.Name, d);
                    new_virtual_drivers.Add(new VirtualDriver(this, ce.Name, (ushort)number, polarity));
                }
            }

            //process and add leds
            if (_machineType == MachineType.PDB && _leds != null)
            {
                ushort i = 0;
                foreach (LedConfigFileEntry le in config.PRLeds)
                {
                    LED led = new LED(this, le.Name, i, le.Number);
                    string number;
                    number = le.Number;
                    led.Polarity = le.Polarity;
                    _leds.Add(i, led.Name, led);
                    i++;
                }
            }

            //TODO: add steppers

            if (_switches != null)
            {
                foreach (SwitchConfigFileEntry se in config.PRSwitches)
                {
                    //Log (se.Number);                    
                    ushort number = 0;
                    if (_machineType == MachineType.PDB)
                    {
                        var num = pdb_config.GetProcNumber("PRSwitches", se.Number);
                        if (num == -1)
                        {
                            Console.WriteLine("Switch {0} cannot be controlled by the P-ROC. Ignoring...", se.Name);
                            continue;
                        }
                        else
                        {
                            number = Convert.ToUInt16(num);
                        }
                    }
                    else
                    {
                        number = PinProcDecoder.PRDecode(_machineType, se.Number);
                    }
                    
                    var s = new Switch(this, se.Name, number, se.Type);
                    s.Number = number;
                    SwitchUpdateRule(number,
                        EventType.SwitchClosedDebounced,
                        new SwitchRule { NotifyHost = true, ReloadActive = false },
                        null,
                        false
                    );
                    SwitchUpdateRule(number,
                        EventType.SwitchOpenDebounced,
                        new SwitchRule { NotifyHost = true, ReloadActive = false },
                        null,
                        false
                    );
                    Logger?.Log("Adding switch " + s.ToString(), LogLevel.Verbose);
                    _switches.Add(s.Number, s.Name, s);
                }

                // TODO: THIS SHOULD RETURN A LIST OF STATES
                EventType[] states = SwitchGetStates();
                foreach (Switch s in _switches.Values)
                {
                    s.SetState(states[s.Number] == EventType.SwitchClosedDebounced);
                }
            }

            if(_lamps != null)
            {
                foreach (LampConfigFileEntry le in config.PRLamps)
                {
                    Driver d;
                    int number;
                    if (_machineType == MachineType.PDB && pdb_config != null)
                    {
                        number = pdb_config.GetProcNumber("PRLamps", le.Number);

                        if (number == -1)
                        {
                            Console.WriteLine("Lamp {0} cannot be controlled by the P-ROC. Ignoring...", le.Name);
                            continue;
                        }
                    }
                    else
                        number = PinProcDecoder.PRDecode(_machineType, le.Number);

                    if ((le.Bus != null && le.Bus == "AuxPort") || number >= PinProcConstants.kPRDriverCount)
                    {
                        d = new VirtualDriver(this, le.Name, (ushort)number, polarity);
                        new_virtual_drivers.Add((VirtualDriver)d);
                    }
                    else
                    {
                        d = new Driver(this, le.Name, (ushort)number);
                        Logger?.Log("Adding driver " + d.ToString(), LogLevel.Verbose);
                        d.ReConfigure(le.Polarity);
                    }
                    _lamps.Add(d.Number, d.Name, d);
                    new_virtual_drivers.Add(new VirtualDriver(this, le.Name, (ushort)number, polarity));
                }
            }

            if (_gi != null)
            {
                foreach (GIConfigFileEntry ge in config.PRGI)
                {
                    Driver d = new Driver(this, ge.Name, PinProcDecoder.PRDecode(_machineType, ge.Number));
                    Logger?.Log("Adding GI " + d.ToString(), LogLevel.Verbose);
                    _gi.Add(d.Number, d.Name, d);
                }
            }

            //replaces _coils, _lamps with virtual drivers
            foreach (VirtualDriver virtual_driver in new_virtual_drivers)
            {
                int base_group_number = virtual_driver.Number / 8;
                List<Driver> items_to_remove = new List<Driver>();

                if (_coils?.Values != null)
                {
                    foreach (Driver d in _coils?.Values)
                    {
                        if (d.Number / 8 == base_group_number)
                            items_to_remove.Add(d);
                    }
                    foreach (Driver d in items_to_remove)
                    {                        
                        _coils.Remove(d.Name);
                        VirtualDriver vd = new VirtualDriver(this, d.Name, d.Number, polarity);
                        _coils.Add(d.Number, d.Name, vd);
                    }
                }                               
                items_to_remove.Clear();

                if(_lamps?.Values != null)
                {
                    foreach (Driver d in _lamps?.Values)
                    {
                        if (d.Number / 8 == base_group_number)
                            items_to_remove.Add(d);
                    }
                    foreach (Driver d in items_to_remove)
                    {
                        _lamps.Remove(d.Name);
                        VirtualDriver vd = new VirtualDriver(this, d.Name, d.Number, polarity);
                        _lamps.Add(d.Number, d.Name, vd);
                    }
                }                
            }
        }
        /// <summary>
        /// Set None to every switch
        /// </summary>
        /// <returns></returns>
        public EventType[] SwitchGetStates()
        {
            EventType[] result = new EventType[256];
            for (int i = 0; i < 256; i++)
                result[i] = EventType.None;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="event_type"></param>
        /// <param name="rule"></param>
        /// <param name="linked_drivers"></param>
        /// <param name="drive_outputs_now"></param>
        public void SwitchUpdateRule(ushort number, EventType event_type, SwitchRule rule, DriverState[] linked_drivers, bool drive_outputs_now)
        {
            int rule_index = ((int)event_type * 256) + number;
            List<IDriver> d = new List<IDriver>();
            if (linked_drivers != null)
            {
                foreach (DriverState s in linked_drivers)
                {
                    d.Add(Drivers[s.DriverNum]);
                }
            }
            if (rule_index >= switch_rules.Length)
                return;
            this.switch_rules[rule_index] = new FakeSwitchRule() { Drivers = d, NotifyHost = rule.NotifyHost };
        }

        /// <summary>
        /// Ticks the virtual Drivers
        /// </summary>
        public void WatchDogTickle()
        {
            foreach (IVirtualDriver d in this.Drivers.Values) d.Tick();
        }

        /// <summary>
        /// Writes data to board, in this Fake case always success
        /// </summary>
        /// <param name="module"></param>
        /// <param name="startingAddr"></param>
        /// <param name="data"></param>
        /// <returns><see cref="Result.Success"/></returns>
        public Result WriteData(uint module, uint startingAddr, ref uint data) => Result.Success;
    }
}
