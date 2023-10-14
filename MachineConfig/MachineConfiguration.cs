using NetPinProc.Domain.MachineConfig;
using NetPinProc.Domain.PinProc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace NetPinProc.Domain
{
    /// <summary>
    /// Machine item types, flagged
    /// </summary>
    [Flags]
    public enum MachineItemType
    {
        /// <summary>
        /// Switch in machine
        /// </summary>
        Switch = 0 << 1,
        /// <summary>
        /// Lamp, on / off
        /// </summary>
        Lamp = 2 << 1,
        /// <summary>
        /// RGB led
        /// </summary>
        Led = 3 << 1,
        /// <summary>
        /// Solenoid / Coil
        /// </summary>
        Coil = 4 << 1,
        /// <summary>
        /// General Illumination
        /// </summary>
        GI = 5 << 1,
        /// <summary>
        /// Stepper motors for PD-LED boards
        /// </summary>
        Stepper = 6 << 1,
        /// <summary>
        /// Servos for PD-LED boards
        /// </summary>
        Servo = 6 << 1,
    }

    /// <summary>
    /// Aggregate class representation of all elements in the configuration file in their deserialized form.
    /// TODO: Dave: PRBallSave is missing, this could have been turned into BallSearch. Was part of the Yaml config
    /// </summary>
    public class MachineConfiguration
    {
        /// <inheritdoc/>
        public MachineConfiguration() { }

        /// <inheritdoc/>
        public BallSearchConfigFileEntry PRBallSearch { get; set; } = new BallSearchConfigFileEntry();

        /// <summary>
        /// These switches should have a coil of the same name. When the switch is active it will fire that coil without code. <para/>
        /// Coils are disabled when flippers are set to disabled in game. <para/>
        /// This works by setting SwitchUpdateRule to the coil
        /// </summary>
        public List<string> PRBumpers { get; set; } = new List<string>();

        /// <inheritdoc/>
        public List<CoilConfigFileEntry> PRCoils { get; set; } = new List<CoilConfigFileEntry>();

        /// <inheritdoc/>
        public List<DriverAliasEntry> PRDriverAliases { get; set; } = new List<DriverAliasEntry>();

        /// <inheritdoc/>
        public PRDriverGlobalsEntry PRDriverGlobals { get; set; } = new PRDriverGlobalsEntry();

        /// <summary> Used in WPC machines MachineType.WPC MachineType.WPC95 MachineType.WPCAlphanumeric</summary>
        public List<string> PRFlipperLeft { get; set; } = new List<string>();

        /// <summary> Used in WPC machines MachineType.WPC MachineType.WPC95 MachineType.WPCAlphanumeric</summary>
        public List<string> PRFlipperRight { get; set; } = new List<string>();

        /// <summary>
        /// Flipper coils should match these switches and be named with Main or Hold eg: switch: flipperLwL and coils: flipperLwLMain, flipperLwLHold <para/>
        /// </summary>
        public List<string> PRFlippers { get; set; } = new List<string>();

        /// <inheritdoc/>
        public GameConfigFileEntry PRGame { get; set; }
        /// <inheritdoc/>
        public List<GIConfigFileEntry> PRGI { get; set; } = new List<GIConfigFileEntry>();

        /// <inheritdoc/>
        public List<LampConfigFileEntry> PRLamps { get; set; } = new List<LampConfigFileEntry>();

        /// <inheritdoc/>
        public List<LedConfigFileEntry> PRLeds { get; set; } = new List<LedConfigFileEntry>();

        /// <inheritdoc/>
        public List<SwitchConfigFileEntry> PRSwitches { get; set; } = new List<SwitchConfigFileEntry>();

        /// <inheritdoc/>
        public List<StepperConfigFileEntry> PRSteppers { get; set; } = new List<StepperConfigFileEntry>();

        /// <inheritdoc/>
        public List<ServoConfigFileEntry> PRServos { get; set; } = new List<ServoConfigFileEntry>();

        /// <inheritdoc/>
        public List<WS281xConfigFileEntry> PRWs281x { get; set; } = new List<WS281xConfigFileEntry>();

        /// <inheritdoc/>
        public List<Lpd8806ConfigFileEntry> PRLpd8806 { get; set; } = new List<Lpd8806ConfigFileEntry>();


        /// <summary>
        /// Initialize configuration from a JSON file on disk
        /// </summary>
        /// <param name="PathToFile">The file to deserialize</param>
        /// <returns>A MachineConfiguration object deserialized from the specified JSON file</returns>
        public static MachineConfiguration FromFile(string PathToFile)
        {
            StreamReader streamReader = new StreamReader(PathToFile);
            string text = streamReader.ReadToEnd();
            return FromJSON(text);
        }

        /// <summary>
        /// Initialize configuration from a string of JSON code
        /// </summary>
        /// <param name="JSON">JSON serialized MachineConfiguration data</param>
        /// <returns>A deserialized MachineConfiguration object</returns>
        public static MachineConfiguration FromJSON(string JSON)
        {
            return JsonSerializer.Deserialize<MachineConfiguration>(JSON, new JsonSerializerOptions()
            { WriteIndented = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true, PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Add a coil to the configuration
        /// </summary>
        /// <param name="Name">Coil name</param>
        /// <param name="Number">Pretty undecoded coil number</param>
        /// <param name="pulseTime">Default pulse time of this coil (30ms by default)</param>
        public void AddCoil(string Name, string Number, int pulseTime = 30)
        {
            CoilConfigFileEntry ce = new CoilConfigFileEntry();
            ce.Name = Name;
            ce.Number = Number;
            ce.PulseTime = pulseTime;
            PRCoils.Add(ce);
        }

        /// <summary>
        /// Add a GI strand to the configuration
        /// </summary>
        /// <param name="Name">GI strand name</param>
        /// <param name="Number">Pretty undecoded strand number</param>
        public void AddGI(string Name, string Number)
        {
            GIConfigFileEntry ge = new GIConfigFileEntry();
            ge.Name = Name;
            ge.Number = Number;
            PRGI.Add(ge);
        }

        /// <summary>
        /// Add a lamp to the configuration
        /// </summary>
        /// <param name="Name">Lamp Name</param>
        /// <param name="Number">Pretty undecoded lamp number</param>
        public void AddLamp(string Name, string Number)
        {
            LampConfigFileEntry le = new LampConfigFileEntry();
            le.Name = Name;
            le.Number = Number;
            PRLamps.Add(le);
        }

        /// <summary>
        /// Add an led to the configuration
        /// </summary>
        /// <param name="Name">Lamp Name</param>
        /// <param name="Number">Pretty undecoded lamp number</param>
        public void AddLED(string Name, string Number)
        {
            var le = new LedConfigFileEntry();
            le.Name = Name;
            le.Number = Number;
            PRLeds.Add(le);
        }

        /// <summary>
        /// Add a switch to the configuration
        /// </summary>
        /// <param name="Name">Switch Name</param>
        /// <param name="Number">Pretty unencoded switch number</param>
        /// <param name="Type">NO = Normally Open (leaf), NC = Normally Closed (optos)</param>
        public void AddSwitch(string Name, string Number, SwitchType Type = SwitchType.NO)
        {
            SwitchConfigFileEntry se = new SwitchConfigFileEntry();
            se.Name = Name;
            se.Type = Type;
            se.Number = Number;
            PRSwitches.Add(se);
        }

        /// <summary>
        /// adds a stepper to <see cref="PRSteppers"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="boardId"></param>
        /// <param name="speed"></param>
        /// <param name="stepper1"></param>
        public void AddStepper(string name, byte boardId, uint speed, bool stepper1 = true)
        {
            var st = new StepperConfigFileEntry()
            {
                Name = name,
                Speed = speed,
                IsStepper1 = stepper1,
                BoardId = boardId,                 
            };

            PRSteppers.Add(st);
        }

        /// <summary>
        /// Get names from collections from a given tag. If you tag all your trough switch with 'trough' then your switch names will be returned
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="machineItemType"></param>
        /// <returns></returns>
        public string[] GetNamesFromTag(string tag, MachineItemType machineItemType)
        {
            switch (machineItemType)
            {
                case MachineItemType.Switch:
                    return PRSwitches?.Where(x => x.Tags != null && x.Tags.ToLower().Contains(tag.ToLower())).Select(n => n.Name)?.ToArray();
                case MachineItemType.Lamp:
                    return PRLamps?.Where(x => x.Tags != null && x.Tags.ToLower().Contains(tag.ToLower())).Select(n => n.Name)?.ToArray();
                case MachineItemType.Led:
                    return PRLeds?.Where(x => x.Tags != null && x.Tags.ToLower().Contains(tag.ToLower())).Select(n => n.Name)?.ToArray();
                case MachineItemType.Coil:
                    return PRCoils?.Where(x => x.Tags != null && x.Tags.ToLower().Contains(tag.ToLower())).Select(n => n.Name)?.ToArray();
                case MachineItemType.GI:
                    return PRGI?.Where(x => x.Tags != null && x.Tags.ToLower().Contains(tag.ToLower())).Select(n => n.Name)?.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Returns the first result of a tag. Use for single switches or coils like used in the trough
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="machineItemType"></param>
        /// <returns></returns>
        public string GetNameFromTag(string tag, MachineItemType machineItemType)
        {
            var names = GetNamesFromTag(tag, machineItemType);
            return names?.Length > 0 ? names[0] : null;
        }

        /// <summary>
        /// Convert the entire MachineConfiguration to XML code and save to a file
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void SaveAsXML(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MachineConfiguration));
            TextWriter textWriter = new StreamWriter(filename, false);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }

        /// <summary>
        /// Convert the entire MachineConfiguration to JSON code
        /// </summary>
        /// <returns>Pretty formatted JSON code</returns>
        public string ToJSON()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
