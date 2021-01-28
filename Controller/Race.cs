using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Timers;
using static Model.Section;
using Timer = System.Timers.Timer;

namespace Controller
{
    public class Race
    {
        public Track Track { get; set; }
        public int NumberOfLaps { get; set; }
        public List<IParticipant> Participants;
        public int ParticipantsOnTrackCount { get; set; }
        public DateTime StartTime { get; set; }
        private Dictionary<Section, SectionData> _positions;
        private Dictionary<IParticipant, LinkedListNode<Section>> _driverWithSection;
        private Dictionary<IParticipant, int> _driverLapCount;
        private Dictionary<IParticipant, int> _driverWithScore;
        private Dictionary<IParticipant, TimeSpan> _driverWithTime;
        private Random _random;

        private Timer _timer;
        public event EventHandler<DriversChangedEventArgs> DriversChanged;
        public event EventHandler NextRace;
        public bool RaceEnded { get; set; }
        public SectionData GetSectionData(Section section)
        { 
            if (_positions.ContainsKey(section))
            {
                return _positions[section];
            }
            else 
            {
               _positions.Add(section, new SectionData());
                return _positions[section];
                    }
        }

        public Race(Track track, List<IParticipant> participants)
        {
            Track = track;
            NumberOfLaps = 0;

            Participants = participants;
            ParticipantsOnTrackCount = Participants.Count;
            StartTime = DateTime.Now;

            _positions = new Dictionary<Section, SectionData>();
            //Dictionary which tracks SectionType Nodes for each driver
            _driverWithSection = new Dictionary<IParticipant, LinkedListNode<Section>>();
            _driverLapCount = new Dictionary<IParticipant, int>();
            _driverWithScore = new Dictionary<IParticipant, int>();
            _driverWithTime = new Dictionary<IParticipant, TimeSpan>();
            InitialiseDictionaries();

            RaceEnded = false;

            // Timer for movement of drivers
            _timer = new Timer(500);
            _timer.Elapsed += OnTimedEvent;

            CalculateStartingPostions();
            //Start();
        }

        public void Start()
        {
            // _timer.Elapsed += OnTimedEvent;
            // _timer.AutoReset = true;
            //_timer.Enabled = true;
            _timer.Start();
        }

        public void Stop()
        {
            //  _timer.Elapsed -= OnTimedEvent;
            // _timer.Enabled = false;
            _timer.Stop();
        }

        public void RandomizeEquipment()
        {
         

       _random = new Random(DateTime.Now.Millisecond);
            foreach (var participant in Participants)
            {
                participant.Equipment.Performance = _random.Next(7, 10);
                participant.Equipment.Quality = _random.Next(1, 3);
                participant.Equipment.Speed = _random.Next(5, 10);
            }
        }

        public int CalculateDriverMovement(IParticipant driver)
        {
            IEquipment equipment = driver.Equipment;
            return equipment.Speed * equipment.Performance;
        }
        public void CalculateStartingPostions()
        {
            int numberOfParticipants = Participants.Count;
            int count = 0;
            for (LinkedListNode<Section> node = Track.Sections.First; node != null; node = node.Next)
            {
                var trackSection = node.Value;
                GetSectionData(trackSection);
                if (trackSection.SectionType == SectionTypes.StartGrid && numberOfParticipants > count)
                {
                    if ((numberOfParticipants - count) % 2 == 0)
                    {
                        _positions[trackSection] = new SectionData(Participants[count], 0, Participants[count + 1], 0);
                        _driverWithSection.Add(Participants[count], node);
                        _driverWithSection.Add(Participants[count + 1], node);
                        _driverLapCount.Add(Participants[count], 0);
                        _driverLapCount.Add(Participants[count + 1], 0);
                       count += 2;
                    }
                    else
                    {
                        _positions[trackSection] = new SectionData(Participants[count], 0, Participants[count + 1], 0);
                        _driverWithSection.Add(Participants[count], node);
                        _driverLapCount.Add(Participants[count], 0);
                        count++;
                    }
                }
            }
        }
        public void InitialiseDictionaries()
        {
            foreach (var driver in Participants)
            {
                _driverWithScore.Add(driver, 0);
                _driverWithTime.Add(driver, new TimeSpan());
            }
        }
        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        
        {
            Debug.WriteLine("OnTimedEvent: " + Track.Name + ": Participant count:" + ParticipantsOnTrackCount);
            if (ParticipantsOnTrackCount > 0)
            {
                if (MoveDrivers(e.SignalTime))
                {
                    DriversChanged?.Invoke(this, new DriversChangedEventArgs() { Track = Track });
                    Debug.WriteLine("Race.OnTimedEvent: Moved Drivers");
                }
            }
            else
            {
                Stop();
                RaceEnded = true;
                Debug.WriteLine("Race.OnTimedEvent: Track has ended");
                NextRace?.Invoke(this, new EventArgs());
                DisposeEvents();
            }
            Debug.WriteLine("Driver event raised: {0:mm:ss.f}",
                e.SignalTime);

        }
        public void DisposeEvents()
        {
            DriversChanged = null;
            NextRace = null;
        }


        public bool MoveDrivers(DateTime time)
        {
            bool driverMoved = false;
            int sectionLenght = 100;
            // decrement to move the front driver first
            for (int i = Participants.Count - 1; i >= 0; i--)
            {
                IParticipant driver = Participants[i];

                int calculatedDistance = CalculateDriverMovement(driver);
                int currentDistance = GetDriverDistance(driver);
                int distance = calculatedDistance + currentDistance;

                if (distance >= sectionLenght)
                {
                    if (MoveDriver(driver, time))
                    {
                        distance -= sectionLenght;
                        UpdateDriverDistance(driver, distance);
                        driverMoved = true;
                    }
                }
                else
                {
                    UpdateDriverDistance(driver, distance);
                    //TODO Bug: wanneer 1 driver over de finish gaat stopt de ander vandaar deze driverMoved op true
                    driverMoved = true;
                }
            }
            return driverMoved;
        }

        public bool MoveDriver(IParticipant driver, DateTime time)
        {
            //Node ophalen waar de driver momenteel opstaat
            LinkedListNode<Section> node = _driverWithSection[driver];
            var sectionData = GetSectionData(node.Value);

            //sectionData ophalen van de volgende node
            LinkedListNode<Section> nextNode;
            if (node.Next != null)
            {
                nextNode = node.Next;
            }
            else
            {
                //Driver reached end of track. Increment lapcount for driver
                nextNode = Track.Sections.First;
                if (!(IncrementDriverLapCount(driver)))
                {
                    RemoveDriverFrom_Positions(driver);
                    SetDriverEndTime(driver, time);
                }
            }

            var sectionDataNextNode = GetSectionData(nextNode.Value);

            SectionData sectionDataPrevious = new SectionData() { Left = null, DistanceLeft = 0, Right = null, DistanceRight = 0 };
            SectionData sectionDataNext = new SectionData() { Left = null, DistanceLeft = 0, Right = null, DistanceRight = 0 };

            //CHECK PREVIOUS SECTIONDATA
            if (sectionData.Left == driver)
            {
                sectionDataNext.Left = driver;
                sectionDataNext.DistanceLeft = sectionData.DistanceLeft;
                //Checks if driver was on other side
                if (sectionData.Right != null)
                {
                    sectionDataPrevious.Right = sectionData.Right;
                    sectionDataPrevious.DistanceRight = sectionData.DistanceRight;
                }
                //check if there is a driver on next section TODO CHECK FOR DRIVER ON LEFT SIDE, WHAT TODO WHEN DRIVER CAN'T MOVE?
                if (sectionDataNextNode.Right != null)
                {
                    sectionDataNext.Right = sectionDataNextNode.Right;
                    sectionDataNext.DistanceRight = sectionDataNextNode.DistanceRight;
                }
            }
            else if (sectionData.Right == driver)
            {
                sectionDataNext.Right = driver;
                sectionDataNext.DistanceRight = sectionData.DistanceRight;
                //Checks if driver was on other side
                if (sectionData.Left != null)
                {
                    sectionDataPrevious.Left = sectionData.Left;
                    sectionDataPrevious.DistanceLeft = sectionData.DistanceLeft;
                }
                //check if there is a driver on next section TODO CHECK FOR DRIVER ON RIGHT SIDE, WHAT TODO WHEN DRIVER CAN'T MOVE?
                if (sectionDataNextNode.Left != null)
                {
                    sectionDataNext.Left = sectionDataNextNode.Left;
                    sectionDataNext.DistanceLeft = sectionDataNextNode.DistanceLeft;
                }
            }

            SetSectionData(node.Value, sectionDataPrevious);
            SetSectionData(nextNode.Value, sectionDataNext);

            _driverWithSection[driver] = nextNode;
            return true;



        }

        public void UpdateDriverScore(IParticipant driver)
        {
            int participantCount = Participants.Count;
            if (ParticipantsOnTrackCount == participantCount)
            {
                _driverWithScore[driver] += 40;
            }
            else if (ParticipantsOnTrackCount == (participantCount - 1))
            {
                _driverWithScore[driver] += 20;
            }
            else if (ParticipantsOnTrackCount == (participantCount - 2))
            {
                _driverWithScore[driver] += 5;
            }
        }
        public void RemoveDriverFrom_Positions(IParticipant driver)
        {
            SectionData sectionData = GetSectionDataByDriver(driver);


            if (sectionData.Left == driver)
            {
                if (sectionData.Right != null)
                {

                    UpdateDriverScore(driver);
                    ParticipantsOnTrackCount -= 1;
                    UpdateDriverScore(sectionData.Right);
                    ParticipantsOnTrackCount -= 1;
                }
                else
                {
                    UpdateDriverScore(driver);
                    ParticipantsOnTrackCount -= 1;
                }
                sectionData.Left = null;
                sectionData.DistanceLeft = 0;
            }
            else
            {
                if (sectionData.Left != null)
                {
                    UpdateDriverScore(driver);
                    ParticipantsOnTrackCount -= 1;
                    UpdateDriverScore(sectionData.Left);
                    ParticipantsOnTrackCount -= 1;
                }
                else
                {
                    UpdateDriverScore(driver);
                    ParticipantsOnTrackCount -= 1;
                }
                sectionData.Right = null;
                sectionData.DistanceRight = 0;
            }
            SetSectionData(_driverWithSection[driver].Value, sectionData);
        }

        public int GetDriverDistance(IParticipant driver)
        {
            int result;
            SectionData sectionData = GetSectionDataByDriver(driver);
            if (sectionData.Left == driver)
            {
                result = sectionData.DistanceLeft;
            }
            else
            {
                result = sectionData.DistanceRight;
            }
            return result;
        }

        public SectionData GetSectionDataByDriver(IParticipant driver)
        {
            Section section = _driverWithSection[driver].Value;
            return GetSectionData(section);
        }

        public void SetSectionData(Section section, SectionData sectionData)
        {
            try
            {
                _positions[section] = sectionData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool UpdateDriverDistance(IParticipant driver, int distanceMoved)
        {
            bool updatedDriver = false;
            Section section = _driverWithSection[driver].Value;
            SectionData sectionData = GetSectionData(section);

            if (sectionData.Right == driver)
            {
                sectionData.DistanceRight = distanceMoved;
                SetSectionData(section, sectionData);
                updatedDriver = true;
            }
            else if (sectionData.Left == driver)
            {
                sectionData.DistanceLeft = distanceMoved;
                SetSectionData(section, sectionData);
                updatedDriver = true;
            }
            return updatedDriver;
        }

        public bool IncrementDriverLapCount(IParticipant driver)
        {
            bool result;
            if (_driverLapCount[driver] < NumberOfLaps)
            {
                _driverLapCount[driver] += 1;
                result = true;
            }
            else
            {
                //Driver finished all laps
                result = false;
            }
            return result;
        }
        public void Dispose()
        {
            DriversChanged = null;
            NextRace = null;
            _timer.Dispose();
        }

        public void SetDriverEndTime(IParticipant driver, DateTime time)
        {
            _driverWithTime[driver] = time - StartTime;
        }

        public Dictionary<IParticipant, int> GetDriversWithScore()
        {
            return _driverWithScore;
        }

        public Dictionary<IParticipant, TimeSpan> GetDriversWithTime()
        {
            return _driverWithTime;
        }

    }
}
