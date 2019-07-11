using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;

namespace Vier_Gewinnt.ViewModel
{
    class vmMainWindow:ViewModelBase
    {

        #region Design..

        private SolidColorBrush ColorRasterField = new SolidColorBrush(Color.FromArgb(255, 0, 100, 200));

        public SolidColorBrush ColorRaster
        {
            get
            {
                return ColorRasterField;
            }

            set
            {
                ColorRasterField = value;
                RaisePropertyChangedEvent(nameof(ColorRaster));
            }
        }

        private SolidColorBrush ColorEinwurfField = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public SolidColorBrush ColorEinwurf
        {
            get
            {
                return ColorEinwurfField;
            }

            set
            {
                ColorEinwurfField = value;
                RaisePropertyChangedEvent(nameof(ColorEinwurf));
            }
        }

        private SolidColorBrush ColorPlayerField = new SolidColorBrush(Color.FromArgb(255,255,0,0));

        public SolidColorBrush ColorPlayer
        {
            get
            {
                return ColorPlayerField;
            }

            set
            {
                ColorPlayerField = value;
                RaisePropertyChangedEvent(nameof(ColorPlayer));
            }
        }

        private SolidColorBrush ColorCPUField = new SolidColorBrush(Color.FromArgb(255, 255, 190, 0));

        public SolidColorBrush ColorCPU
        {
            get
            {
                return ColorCPUField;
            }

            set
            {
                ColorCPUField = value;
                RaisePropertyChangedEvent(nameof(ColorCPU));
            }
        }

        private System.Windows.Media.Effects.DropShadowEffect DSRasterField = new System.Windows.Media.Effects.DropShadowEffect();

        public DropShadowEffect DSRaster
        {
            get
            {
                return DSRasterField;
            }

            set
            {
                DSRasterField = value;
                RaisePropertyChangedEvent(nameof(DSRaster));
            }
        }

        private System.Windows.Media.Effects.DropShadowEffect DSCoinField = new System.Windows.Media.Effects.DropShadowEffect();

        public DropShadowEffect DSCoin
        {
            get
            {
                return DSCoinField;
            }

            set
            {
                DSCoinField = value;
                RaisePropertyChangedEvent(nameof(DSCoin));
            }
        }



        #endregion

        #region Felder..

        private System.Windows.Controls.Canvas[,] FelderField = new System.Windows.Controls.Canvas[7,6];

        public Canvas[,] Felder
        {
            get
            {
                return FelderField;
            }

            set
            {
                FelderField = value;
                RaisePropertyChangedEvent(nameof(Felder));
            }
        }

        private System.Windows.Controls.Canvas[,] FelderEinwurfField = new System.Windows.Controls.Canvas[7, 1];

        public Canvas[,] FelderEinwurf
        {
            get
            {
                return FelderEinwurfField;
            }

            set
            {
                FelderEinwurfField = value;
                RaisePropertyChangedEvent(nameof(FelderEinwurf));
            }
        }

        private System.Windows.Size FieldSize { get; set; }

        private Model.Connect4Board GameBoard;

        private DispatcherTimer Timer = new DispatcherTimer();

        private int SpielSekundenField;

        public int SpielSekunden
        {
            get
            {
                return SpielSekundenField;
            }

            set
            {
                SpielSekundenField = value;
                RaisePropertyChangedEvent(nameof(SpielSekunden));
            }
        }

        private bool InputEnabledField = false;

        public bool InputEnabled
        {
            get
            {
                return InputEnabledField;
            }

            set
            {
                InputEnabledField = value;
                //System.Diagnostics.Debug.Print("Input Enabled = " + value);
                RaisePropertyChangedEvent(nameof(InputEnabled));
            }
        }

        SoundPlayer soundPlayer = new SoundPlayer(Properties.Resources.Klick);

        #endregion

        public vmMainWindow()
        {
            InitFelder();

            ACSpielStart = new MyCommand();
            ACSpielStart.CanExecuteFunc = obj => true;
            ACSpielStart.ExecuteFunc = ACSpielStartFunc;

            ACBeenden = new MyCommand();
            ACBeenden.CanExecuteFunc = obj => true;
            ACBeenden.ExecuteFunc = ACBeendenFunc;

            //DSRaster.BlurRadius = 5;
            //DSRaster.Direction = 45;
            //DSRaster.ShadowDepth = 2;
            //DSRaster.Opacity = 0.7;

            DSRaster.BlurRadius = 1;
            DSRaster.Direction = 45;
            DSRaster.ShadowDepth = 2;
            DSRaster.Opacity = 0.5;

            DSCoin.BlurRadius = 5;
            DSCoin.Direction = 200;
            DSCoin.ShadowDepth = 2;
            DSCoin.Opacity = 0.5;
            DSCoin.Color = Color.FromArgb(255, 0, 0, 0);

            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
        }

        #region Events..

        private void Timer_Tick(object sender, EventArgs e)
        {
            SpielSekunden++;
            //System.Diagnostics.Debug.Print(SpielSekunden + " Sekunden");
        }

        private void Felder_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            FieldSize = e.NewSize;
        }

        private void FeldEinwurf_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(InputEnabled)
            { 
                Canvas c = (Canvas)sender;
                int Index = (int)c.Tag;

                if (c.Children.Count > 0){ c.Children[0].Visibility = Visibility.Hidden;}

                DoEvents();

                ZugSetzen(true,Index);
                ZugSetzen(false,Index);

                if (c.Children.Count > 0){c.Children[0].Visibility = Visibility.Visible; c.InvalidateVisual(); DoEvents(); }

                if (!Timer.IsEnabled && GameBoard.endt == 0) {Timer.Start();}
            }

        }

        private void FeldEinwurf_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Canvas c = (Canvas)sender;
            c.Children.Clear();
        }

        private void FeldEinwurf_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(InputEnabled)
            { 
                Canvas c = (Canvas)sender;
                c.Children.Clear();
                c.Children.Add(GetCoin(ColorPlayer,"P"));
                //System.Diagnostics.Debug.Print(c.Name);
            }
        }

        private void Spiel_EinEvent(object sender, Model.GameOverEventArgs e)
        {
            Timer.Stop();
            InputEnabled = false;

            Model.Connect4Board.ePlayer Winner = (Model.Connect4Board.ePlayer)e.Number;

            string msg = string.Empty;

            switch (Winner)
            {
                case Model.Connect4Board.ePlayer.Human:
                    msg = "Sie haben gewonnen..";
                    soundPlayer = new SoundPlayer(Properties.Resources.Applaus);
                    break;
                case Model.Connect4Board.ePlayer.Computer:
                    msg = "Sie haben verloren..";
                    soundPlayer = new SoundPlayer(Properties.Resources.Lose);
                    break;
                case Model.Connect4Board.ePlayer.None:
                    msg = "unentschieden..";
                    break;
                default:
                    break;
            }

            soundPlayer.Play();

            DoEvents();

            System.Windows.MessageBox.Show(msg,"Spielende", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Methoden..

        private void InitFelder()
        {
            for (int x = 0; x < 7; x++)
            {
                FelderEinwurf[x, 0] = new Canvas();
                FelderEinwurf[x, 0].Name = "EW" + x.ToString();
                FelderEinwurf[x, 0].Tag = x;
                FelderEinwurf[x, 0].Margin = new System.Windows.Thickness(4);//6
                FelderEinwurf[x, 0].Background = ColorEinwurf;


                FelderEinwurf[x, 0].MouseEnter += FeldEinwurf_MouseEnter;
                FelderEinwurf[x, 0].MouseLeave += FeldEinwurf_MouseLeave;
                FelderEinwurf[x, 0].PreviewMouseUp += FeldEinwurf_PreviewMouseUp;
            }

            RaisePropertyChangedEvent(nameof(FelderEinwurf));

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    Felder[x, y] = new Canvas();
                    Felder[x, y].Margin = new System.Windows.Thickness(4);//6
                    Felder[x, y].Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 200, 200, 200));
                    //Felder[x, y].Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 155, 255));
                    Felder[x, y].Effect = DSRaster;
                    Felder[x, y].SizeChanged += Felder_SizeChanged;
                }
            }

            RaisePropertyChangedEvent(nameof(Felder));
        }

        private Grid GetCoin(SolidColorBrush Farbe, string Text)
        {
            Grid Container = new Grid();

            Ellipse Coin = new Ellipse();
            Coin.Width = FieldSize.Width;
            Coin.Height = FieldSize.Height;
            Coin.Fill = Farbe;
            Coin.Stroke = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
            Coin.StrokeThickness = 1;
            Coin.UseLayoutRounding = true;
            Coin.Effect = DSCoin;

            Ellipse CoinInner = new Ellipse();
            CoinInner.Width = FieldSize.Width / 1.4;
            CoinInner.Height = FieldSize.Height / 1.4;
            CoinInner.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
            CoinInner.Stroke = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            CoinInner.StrokeThickness = 1;
            CoinInner.UseLayoutRounding = true;

            Label tbx = new Label();
            tbx.Padding = new System.Windows.Thickness(0);
            tbx.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            tbx.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

            tbx.Content = Text;
            tbx.FontFamily = new FontFamily("Consolas");
            tbx.FontWeight = System.Windows.FontWeights.Bold;
            tbx.FontSize = 40;
            tbx.Foreground = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));


            Container.Children.Add(Coin);
            Container.Children.Add(CoinInner);
            Container.Children.Add(tbx);

            return Container;

        }

        private void CoinEinwurf(int Spalte, SolidColorBrush ColorCoin, string Text, bool CPU)
        {

            if (Felder[Spalte, 0].Children.Count == 0)
            {
                for (int i = 5; i >= 0; i--)
                {
                    if (Felder[Spalte, i].Children.Count == 0)
                    {
                        if (CPU)
                        {
                            DoEvents();

                            System.Threading.Thread.Sleep(300);

                            FelderEinwurf[GameBoard.col, 0].Children.Clear();
                            FelderEinwurf[GameBoard.col, 0].Children.Add(GetCoin(ColorCPU, "C"));

                            DoEvents();

                            System.Threading.Thread.Sleep(300);

                            FelderEinwurf[Spalte, 0].Children.Clear();
                        }

                        Felder[Spalte, i].Children.Add(GetCoin(ColorCoin, Text));


                        break;
                    }
                }
            }

            System.Diagnostics.Debug.Print("Einwurf "+ Spalte);
        }

        private void ZugSetzen(bool Spieler, int Spalte)
        {
            InputEnabled = false;

            if (GameBoard.endt == 0)
            {
                if (Spieler)
                {
                    CoinEinwurf(Spalte, ColorPlayer, "P", false);

                    if (GameBoard.endt == 0)
                    {
                        InputEnabled = false;

                        if (GameBoard.add(Spalte, GameBoard.plr) == 0)
                        {
                            GameBoard.turncheck();
                        }
                    }

                    GameBoard.turn = GameBoard.cpu;
                }
                else//CPU zieht
                {

                    GameBoard.col = GameBoard.Think();

                    CoinEinwurf(GameBoard.col, ColorCPU, "C", true);

                    if (GameBoard.add(GameBoard.col, GameBoard.cpu) == 0)
                    {
                        GameBoard.turncheck();
                    }


                    GameBoard.turn = GameBoard.plr;
                }

                

                if (GameBoard.endt == 0)
                {
                    soundPlayer.Play();
                    InputEnabled = true;
                }
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        #endregion

        #region Aktionen..

        public MyCommand ACSpielStart
        {
            get;
            set;
        }

        public void ACSpielStartFunc(object parameter)
        {
           View.dlgSpielStart ofd = new View.dlgSpielStart();
           ofd.Owner = (System.Windows.Window)parameter;
           ofd.Title = "Neues Spiel starten";

            if ((bool)ofd.ShowDialog())
            {
                InitFelder();

                GameBoard = new Model.Connect4Board();
                GameBoard.GameOverEvent += Spiel_EinEvent;

                SpielSekunden = 0;

                soundPlayer = new SoundPlayer(Properties.Resources.Klick);

                if ((bool)ofd.Spiel_Leicht.IsChecked)
                {
                    //Easy
                    GameBoard.rec = 3;
                }
                else
                {
                    //Hard
                    GameBoard.rec = 5;
                }

                if ((bool)ofd.Start_Spieler.IsChecked)
                {
                    //CPU beginnt
                    GameBoard.turn = GameBoard.player;
                }
                else
                {
                    ////CPU beginnt
                    GameBoard.turn = GameBoard.cpu;

                    ZugSetzen(false, 0);

                    DoEvents();

                    Timer.Start();
                }

                
                InputEnabled = true;
                
            }
        }

        public MyCommand ACBeenden
        {
            get;
            set;
        }

        public void ACBeendenFunc(object parameter)
        {
            if (System.Windows.MessageBox.Show("Programm beenden?", "Benutzerabfrage", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        #endregion

    }
}
