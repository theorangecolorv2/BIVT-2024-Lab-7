using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_7
{
    public class Purple_3
    {

        public struct Participant
        {
            private string _name;
            private string _surname;
            private double[] _marks;
            private int[] _places;
            private int _markIndex;

            public Participant(string name, string surname)
            {
                _name = name;
                _surname = surname;
                _marks = new double[7];
                _places = new int[7];
                _markIndex = 0;
            }

            public string Name => _name;
            public string Surname => _surname;
            public double[] Marks
            {
                get
                {
                    if (_marks == null) return null;
                    double[] marks = new double[_marks.Length];
                    Array.Copy(_marks, marks, _marks.Length);
                    return marks;
                }
            }

            public int[] Places
            {
                get
                {
                    if (_places == null) return null;
                    int[] copy = new int[_places.Length];
                    Array.Copy(_places, copy, _places.Length);

                    return copy;
                }
            }

            public int Score
            {
                get
                {
                    if (_places == null) return 0;

                    return _places.Sum();
                }
            }

            public void Evaluate(double result)
            {
                if (_markIndex >= 7 || _marks == null) return;
                _marks[_markIndex++] = result;
            }

            public static void SetPlaces(Participant[] participants)
            {
                if (participants == null || participants.Length == 0) return;

                for (int judge = 0; judge < 7; judge++)
                {
                    int validCount = 0;

                    foreach (Participant participant in participants)
                    {
                        if (participant.Marks != null && participant.Places != null) validCount++;
                    }

                    Participant[] valid = new Participant[validCount];
                    int index = 0;

                    foreach (Participant participant in participants)
                    {
                        if (participant.Marks != null && participant.Places != null) valid[index++] = participant;
                    }


                    for (int i = 1; i < valid.Length;)
                    {

                        if (i == 0 || valid[i - 1].Marks[judge] >= valid[i].Marks[judge])
                        {
                            i++;
                        }
                        else
                        {
                            Participant temp = valid[i - 1];
                            valid[i - 1] = valid[i];
                            valid[i] = temp;
                            i--;
                        }
                    }

                    for (int i = 0; i < validCount; i++)
                    {
                        valid[i]._places[judge] = i + 1;
                    }

                    if (judge == 6)
                    {
                        index = valid.Length;
                        Participant[] res = new Participant[participants.Length];
                        Array.Copy(valid, res, valid.Length);

                        foreach (var participant in participants)
                        {
                            if (participant.Marks == null) res[index++] = participant;
                        }

                        Array.Copy(res, participants, participants.Length);
                    }
                }
            }




            public static void Sort(Participant[] array) // переписал на пузырьковую, тк она стабильна + исправил вторичные критерии
            {
                if (array == null || array.Length == 0) return;
                
                for (int j = 0; j < array.Length - 1; j++)
                {
                    for (int i = 0; i < array.Length - 1 - j; i++)
                    {
                        Participant current = array[i + 1];
                        Participant previous = array[i];

                        bool currentIsNull = current._places == null || current._marks == null;
                        bool prevIsNull = previous._places == null || previous._marks == null;
                        if (prevIsNull && !currentIsNull)
                        {
                            (array[i], array[i + 1]) = (current, previous);
                            continue;
                        }
                        if (!currentIsNull && !prevIsNull)
                        {
                            if (current.Score < previous.Score)
                            {
                                (array[i], array[i + 1]) = (current, previous);
                                continue;
                            }
                            else if (current.Score == previous.Score)
                            {
                                int currentMinPlace = current._places.Min();
                                int prevMinPlace = previous._places.Min();
                                if (currentMinPlace < prevMinPlace)
                                {
                                    (array[i], array[i + 1]) = (current, previous);
                                    continue;
                                }
                                else if (currentMinPlace == prevMinPlace)
                                {
                                    double currentSum = current._marks.Sum();
                                    double prevSum = previous._marks.Sum();
                                    if (currentSum > prevSum)
                                    {
                                        (array[i], array[i + 1]) = (current, previous);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }



            public void Print()
            {

                Console.Write($"{Name} {Surname}:  ");
                if (Places != null && Places.Length > 0)
                {
                    Console.Write($"{Places.Sum()} ");
                    Console.Write($"{Places.Min()} ");
                }

                //else
                //{
                //    Console.Write("nul null");
                //}

                if (Marks != null && Marks.Length > 0)
                {
                    Console.Write($"{Marks.Sum():F2}");
                }
                //else
                //{
                //    Console.Write("null");
                //}

                Console.WriteLine();
            }





        }


        public abstract class Skating
        {
            private Participant[] _participants;
            protected double[] _moods;

            public Participant[] Participants => _participants;
            public double[] Moods => _moods;


            public Skating(double[] moods) {
                _participants = new Participant[0];
                if (moods == null) return;
                int size;
                if (moods.Length > 7)
                {
                     size = 7;
                }
                else
                {
                     size = moods.Length;
                }

                _moods = new double[size];
                Array.Copy(moods, _moods, size);
                ModificateMood();

            }

            protected abstract void ModificateMood();

            public void Evaluate(double[] marks)
            {
                if (marks == null) return;
                int index = -1;
                for (int i = 0; i < _participants.Length; i++)
                {
                    if (_participants[i].Marks != null)
                    {
                        if (_participants[i].Marks.All(m => m == 0))
                        {
                            index = i; break;
                        }
                    }
                }
                if (index < 0) { return; }

                int size;
                if (marks.Length > 7) { size = 7; }
                else { size = marks.Length; }

                for (int i = 0;i < size; i++)
                {
                    _participants[index].Evaluate(marks[i] * _moods[i]);
                }

            }




            public void Add(Participant participant)
            {
                if (_participants == null) return;
                Array.Resize(ref _participants, _participants.Length + 1);
                _participants[_participants.Length - 1] = participant;
            }

            public void Add(Participant[] participants)
            {
                if (_participants == null || participants == null) return;
                foreach (var participant in participants)
                {
                    Add(participant);
                }
            }
        }

        public class FigureSkating : Skating
        {
            public FigureSkating(double[] moods) : base(moods) { }

            protected override void ModificateMood()
            {
                for (int i = 0; i < _moods.Length; i++)
                {
                    _moods[i] += (i + 1.0) / 10;
                }
            }
        }

        public class IceSkating : Skating
        {
            public IceSkating(double[] moods) : base(moods) { }

            protected override void ModificateMood()
            {
                for (int i = 0; i < _moods.Length; i++)
                {
                    _moods[i] *= (i + 1.0 + 100) / 100;
                }
            }
        }
    }
}