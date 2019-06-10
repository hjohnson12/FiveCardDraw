/*
 * Programmer: Hunter Johnson
 * Info: 5 Card Draw in C# - Ported over from VisualBasic.NET
 * Date: 12/1/16
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker_5CardDraw
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        int intLastCard = 51; // last card in deck
        int[] intPlayerCards = new int[5]; // Players hand
        int[] intDealerCards = new int[5]; // Dealers hand
        int burn; // used to start the random number generator off
        private Random r = new Random();
        // Variables below here are for betting and stats
        double intPlayerWins;
        double intPlayerLoses;
        double dblPlayerRatio=0;
        double dblPlayerWallet = 50; // Starts out at 50 dollars
        double intDealerWins;
        double intDealerLoses;
        double dblDealerRatio = 0;
        double dblDealerWallet = 50; // Starts out at 50 dollars
        double dblJackpot = 1000; //Starts at 150 dollars


        // ************** Deck of Cards ****************************
        int[] intDeck = new int[52] {21, 22, 23, 24, 31, 32, 33, 34, 41, 42, 43, 44, 51, 52, 53, 54, 61, 62, 63, 64, 71, 72, 73, 74,
                                    81, 82, 83, 84, 91, 92, 93, 94, 101, 102, 103, 104, 111, 112, 113, 114, 121, 122, 123,
                                    124, 131, 132, 133, 134, 141, 142, 143, 144};

        // *********************** Menu Exit - Click ************************************
        private void mnuFileExit_Click1(object sender, EventArgs e)
        {
            DialogResult dlgExit;
            // prompt user and ask if they wish to proceed with exiting
            dlgExit = MessageBox.Show("Are you sure you want to exit?", "Exit 5 Card Draw", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            // If user selects yes
            if ((dlgExit == DialogResult.Yes))
            {
                // Exit the game
                this.Close();
            }

        }

        //******************************************************************************************************
        // btnDeal - Shuffles the deck on start, Deals 5 cards to the Player and Dealer, then sorts the hands. *
        //           Pass 0 in the Dealer function to shuffle the deck. Pass 1 to deal a card.                 *
        //******************************************************************************************************
        private void btnDeal_Click(object sender, EventArgs e)
        {
            int i;
            this.Dealer(0);  // Shuffle the deck - Dealer(0)

            // Deal Cards to Player and Dealer
            // 0 is for shuffle deck - Dealer(0)
            // 1 is for replace card - Dealer(1)
            for (i = 0; (i <= 4); i++)
            {
                intPlayerCards[i] = this.Dealer(1);
                intDealerCards[i] = this.Dealer(1);
            }
            lblWinner.Text = "Cards Dealt";
            // Sort Hands
            this.sort(ref intPlayerCards);                                                                                                                                                   
            this.sort(ref intDealerCards);
            picPC1.ImageLocation = Application.StartupPath + "\\Resources\\"+ intPlayerCards[0].ToString() + ".png";
            picPC2.ImageLocation = Application.StartupPath + "\\Resources\\"+ intPlayerCards[1].ToString() + ".png";
            picPC3.ImageLocation = Application.StartupPath + "\\Resources\\"+ intPlayerCards[2].ToString() + ".png";
            picPC4.ImageLocation = Application.StartupPath + "\\Resources\\"+ intPlayerCards[3].ToString() + ".png";
            picPC5.ImageLocation = Application.StartupPath + "\\Resources\\"+ intPlayerCards[4].ToString() + ".png";
            picDC1.ImageLocation = Application.StartupPath + "\\Resources\\" + "back.jpg";
            picDC2.ImageLocation = Application.StartupPath + "\\Resources\\" + "back.jpg";
            picDC3.ImageLocation = Application.StartupPath + "\\Resources\\" + "back.jpg";
            picDC4.ImageLocation = Application.StartupPath + "\\Resources\\" + "back.jpg";
            picDC5.ImageLocation = Application.StartupPath + "\\Resources\\" + "Back.jpg";
            chk1.Show();
            chk2.Show();
            chk3.Show();
            chk4.Show();
            chk5.Show();
            // Enable Checkboxes & Replace Button
            btnReplace.Enabled = true;
            chk1.Enabled = true;
            chk2.Enabled = true;
            chk3.Enabled = true;
            chk4.Enabled = true;
            chk5.Enabled = true;
            // enable betting buttons and display stats
            btnOne.Enabled = true;
            btnFive.Enabled = true;
            btnTen.Enabled = true;
            btnTwentyFive.Enabled = true;
            btnFifty.Enabled = true;
            btnOneHundred.Enabled = true;
            btnFiveHundred.Enabled = true;
            try
            {
                dblPlayerRatio = (intPlayerWins / intPlayerLoses);
                dblDealerRatio = (intDealerWins / intDealerLoses);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Divide By Zero Exception");
            }
            lblPlayerWins.Text = (intPlayerWins.ToString() + "/10");
            lblPlayerLoses.Text = intPlayerLoses.ToString();
            lblPlayerRatio.Text = dblPlayerRatio.ToString("N2");
            lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            lblDealerWins.Text = (intDealerWins.ToString() + "/10");
            lblDealerLoses.Text = intDealerLoses.ToString();
            lblDealerRatio.Text = dblDealerRatio.ToString("N2");
            lblDealerWallet.Text = dblDealerWallet.ToString("C2");
        }

        //*****************************************************************************
        // sort - Accepts an array of cards By Reference.                             *
        //        Compares each value side by side. Places the bigger number in front *
        //*****************************************************************************
        private void sort(ref int[] hand)
        {
            int i;
            int j;
            int intTemp;
            for (i = 0; (i <= 3); i++)
            {
                for (j = (i + 1); (j <= 4); j++)
                {
                    if ((hand[j] > hand[i]))
                    {
                        intTemp = hand[i];
                        hand[i] = hand[j];
                        hand[j] = intTemp;
                    }
                }
            }
        }

        // ********************** Evaluate Hand Function *************************
        public double evaluateHand(ref int[] hand)
        {
            double dblHandResult;
            // Straight Flush   8.0
            dblHandResult = this.straightFlush(ref hand);
            // Four of A Kind   7.0
            if ((dblHandResult == 0))
            {
                dblHandResult = this.fourOfKind(ref hand);
                // Full House   6.0
                if ((dblHandResult == 0))
                {
                    dblHandResult = this.fullHouse(ref hand);
                    // Flush   5.0
                    if ((dblHandResult == 0))
                    {
                        dblHandResult = this.flush(ref hand);
                        // Straight   4.0
                        if ((dblHandResult == 0))
                        {
                            dblHandResult = this.straight(ref hand);
                            // Three of A Kind   3.0
                            if ((dblHandResult == 0))
                            {
                                dblHandResult = this.threeOfKind(ref hand);
                                // Two Pair   2.0
                                if ((dblHandResult == 0))
                                {
                                    dblHandResult = this.twoPair(ref hand);
                                    // One Pair   1.0
                                    if ((dblHandResult == 0))
                                    {
                                        dblHandResult = this.onePair(ref hand);
                                        // High Card    0.0
                                        if ((dblHandResult == 0))
                                        {
                                            dblHandResult = this.highCard(ref hand);
                                        }
                                    }
                                }                            
                            }
                        }
                    }
                }
            }
            return dblHandResult;
        }

        // *********************** High Card Function **************************
        private double highCard(ref int[] Hand)
        {
            int intC1;
            int intC2;
            int intC3;
            int intC4;
            int intC5;
            //  Simulated Array
            intC1 = Hand[0];
            intC2 = Hand[1];
            intC3 = Hand[2];
            intC4 = Hand[3];
            intC5 = Hand[4];
            //  Arrange multipliers
            intC1 = intC1/10;
            intC2 = intC2/10;
            intC3 = intC3/10;
            intC4 = intC4/10;
            intC5 = intC5/10;
            double dblWeighedHand = ((intC1 * 0.01)
                        + ((intC2 * 0.0001)
                        + ((intC3 * 1E-06)
                        + ((intC4 * 1E-08)
                        + (intC5 * 1E-10)))));
            return dblWeighedHand;
        }

        // ************************* One Pair Function ***********************
        private double onePair(ref int[] hands)
        {
            double dblScore;
            int[] hand = new int[5];
            for (int i = 0; (i <= 4); i++)
            {
                hand[i] = (hands[i] / 10);
            }

            if ((hand[0] == hand[1]))
            {
                dblScore = (1+ ((hand[0] * 0.01)+ ((hand[2] * 0.0001)+ ((hand[3] * 1E-06)+ (hand[4] * 1E-08)))));
                return dblScore;
            }
            else if ((hand[1] == hand[2]))
            {
                dblScore = (1+ ((hand[1] * 0.01)+ ((hand[0] * 0.0001)+ ((hand[3] * 1E-06)+ (hand[4] * 1E-08)))));
                return dblScore;
            }
            else if ((hand[2] == hand[3]))
            {
                dblScore = (1+ ((hand[2] * 0.01)+ ((hand[0] * 0.0001)+ ((hand[1] * 1E-06)+ (hand[4] * 1E-08)))));
                return dblScore;
            }
            else if ((hand[3] == hand[4]))
            {
                dblScore = (1+ ((hand[3] * 0.01)+ ((hand[0] * 0.0001)+ ((hand[1] * 1E-06)+ (hand[2] * 1E-08)))));
                return dblScore;
            }
            else
            {
                dblScore = 0;
                return dblScore;
            }

        }
        // ************************* Two Pair Function ********************************
        private double twoPair(ref int[] intCard)
        {
            double dblValue = 0;
            if (intCard[0] / 10 == intCard[1] / 10 & intCard[2] / 10 == intCard[3] / 10)
            {
                dblValue = 2 + (intCard[0] / 10) * 0.01 + (intCard[2] / 10) * 0.0001 + (intCard[4] / 10) * 1E-06;
                return dblValue;
            }
            else if (intCard[0] / 10 == intCard[1] / 10 & intCard[3] / 10 == intCard[4] / 10)
            {
                dblValue = 2 + (intCard[1] / 10) * 0.01 + (intCard[3] / 10) * 0.0001 + (intCard[3] / 10) * 1E-06;
                return dblValue;
            }
            else if (intCard[1] / 10 == intCard[2] / 10 & intCard[3] / 10 == intCard[4] / 10)
            {
                dblValue = 2 + (intCard[2] / 10) * 0.01 + (intCard[3] / 10) * 0.0001 + (intCard[0] / 10) * 1E-06;
                return dblValue;
            }
            else
            {
                return dblValue;
            }
        }
       
        // ******************** Three of A Kind Function *********************
        private double threeOfKind(ref int[] intCard)
        {
            double dblValue;
            dblValue = 0;
            if (intCard[0] / 10 == intCard[2] / 10)
            {
                dblValue = 3 + (intCard[0] / 10) * 0.01;
                return dblValue;
            }
            else if (intCard[1] / 10 == intCard[3] / 10)
            {
                dblValue = 3 + (intCard[1] / 10) * 0.01;
                return dblValue;
            }
            else if (intCard[2] / 10 == intCard[4] / 10)
            {
                dblValue = 3 + (intCard[2] / 10) * 0.01;
                return dblValue;
            }
            else
            {
                return dblValue;

            }

        }

        // ******************* Straight Function ***********************
        private double straight(ref int[] intC)
        {
            //  intC is the name of the array 
            double dblValue = 0;
            // if there is a straight
            if (((intC[0] / 10) - (intC[1] / 10) == 1) && ((intC[1] / 10) - (intC[2] / 10) == 1) && ((intC[2] / 10) - (intC[3] / 10) == 1) && ((intC[3] / 10) - (intC[4] / 10) == 1))
            {
                dblValue = 4.0 + (intC[0] / 10) * 0.01;
            }

            return dblValue;
        }

        // ***************** Flush Function ************************
        private double flush(ref int[] intC)
        {
            //  intC is the name of the array
            double dblValue = 0;
            //  if there is a flush

            if ((intC[0] % 10 == intC[1] % 10) && (intC[1] % 10 == intC[2] % 10) && (intC[2] % 10 == intC[3] % 10) && (intC[3] % 10 == intC[4] % 10))
            {
                dblValue = 5.0 + ((intC[0] / 10) * 0.01) + ((intC[1] / 10) * 0.0001) + ((intC[2] / 10) * 1E-06) + ((intC[3] / 10) * 1E-08) + ((intC[4] / 10) * 1E-10);
            }

            return dblValue;
        }

        // ******************* Full House Function ************************
        private double fullHouse(ref int[] intCard)
        {
            double dblValue;
            dblValue = 0;

            if (intCard[0] / 10 == intCard[2] / 10 & intCard[3] / 10 == intCard[4] / 10)
            {
                dblValue = 6 + (intCard[0] / 10) * 0.01 + (intCard[3] / 10) * 0.0001;
                return dblValue;
            }
            else if (intCard[0] / 10 == intCard[1] / 10 & intCard[2] / 10 == intCard[4] / 10)
            {
                dblValue = 6 + (intCard[0] / 10) * 0.01 + (intCard[2] / 10) * 0.0001;
                return dblValue;
            }
            else
            {
                return dblValue;
            }
        }

        // ***************** Four of A Kind Function ***********************
        private double fourOfKind(ref int[] intCard)
        {
            double dblValue;
            dblValue = 0;

            if (intCard[0] / 10 == intCard[3] / 10)
            {
                dblValue = 7 + (intCard[0] / 10) * 0.01;
                return dblValue;
            }
            else if (intCard[1] / 10 == intCard[4] / 10)
            {
                dblValue = 7 + (intCard[1] / 10) * 0.01;
                return dblValue;
            }
            else
            {
                return dblValue;
            }
        }

        // ******************* Straight Flush Function ***********************
        private double straightFlush(ref int[] intC)
        {
            double dblValue = 0;
            int intCount = 0;
            //  if there is a straight flush

            if (((intC[0] / 10) - (intC[1] / 10) == 1) && ((intC[1] / 10) - (intC[2] / 10) == 1) && ((intC[2] / 10) - (intC[3] / 10) == 1) && ((intC[3] / 10) - (intC[4] / 10) == 1))
            {
                intCount = 1;
            }
            if ((intC[0] % 10 == intC[1] % 10) && (intC[1] % 10 == intC[2] % 10) && (intC[2] % 10 == intC[3] % 10) && (intC[3] % 10 == intC[4] % 10))
            {
                intCount += 1;
            }

            if ((intCount == 2))
            {
                dblValue = 8.0 + (intC[0] / 10) * 0.01;
            }
            return dblValue;
        }

        // ********************************************************************************
        // Dealer - Accepts an int parameter. 0 is for Shuffle, 1 is for Dealing the card *
        //*********************************************************************************
        private int Dealer(int intOP)
        {
            if ((intOP == 0))
            {
                intLastCard = 51;
                return 0;
            }
            else
            {
                int x;
                int intSwap;
                burn = r.Next();
                // Burn a number to start the random generator off
                if ((intLastCard == 0))
                {
                    return 0;
                }

                x = r.Next(0, intLastCard);
                intSwap = intDeck[x];
                // Swap and random generator work
                intDeck[x] = intDeck[intLastCard];
                intDeck[intLastCard] = intSwap;
                intLastCard = (intLastCard - 1);
                return intDeck[intLastCard + 1];
            }

        }

        //***************************************************************************************
        // dealerLogic - Accepts an array of cards, and a dealers hand total as its parameters. *
        //               It then checks the dealers cards, and replaces according to the value. *
        //***************************************************************************************
        public void dealerLogic(ref int[] intDealerCards, double dblDealerHandTotal)
        {
            int[] buffer = new int[4];
            if (true)
            {
                if (dblDealerHandTotal < 1) {
                    buffer[((intDealerCards[0] % 10) - 1)] = buffer[((intDealerCards[0] % 10) - 1)] + 1;
                    buffer[((intDealerCards[1] % 10) - 1)] = buffer[((intDealerCards[1] % 10) - 1)] + 1;
                    buffer[((intDealerCards[2] % 10) - 1)] = buffer[((intDealerCards[2] % 10) - 1)] + 1;
                    buffer[((intDealerCards[3] % 10) - 1)] = buffer[((intDealerCards[3] % 10) - 1)] + 1;
                    buffer[((intDealerCards[4] % 10) - 1)] = buffer[((intDealerCards[4] % 10) - 1)] + 1;

                    for (int i = 0; i <= 3; i++)
                    {
                        if ((buffer[i] >= 3))
                        {
                            for (int j = 0; j <= 4; j++)
                            {
                                if (((intPlayerCards[j] % 10) != 1))
                                {
                                    intPlayerCards[j] = 0;
                                }
                            }
                        }
                    }

                    if (intDealerCards[0] / 10 == ((intDealerCards[3] / 10) + 4))
                    {
                        intDealerCards[4] = 0;
                    }
                    if (intDealerCards[1] / 10 == ((intDealerCards[4] / 10) + 4))
                    {
                        intDealerCards[4] = 0;
                    }

                    if (intDealerCards[2] == 51 | intDealerCards[2]== 52 | intDealerCards[2] == 53 | intDealerCards[2] == 54)
                    {
                        if (intDealerCards[0] == 141 | intDealerCards[0] == 142 | intDealerCards[0] == 143 | intDealerCards[0] == 144)
                        {
                            intDealerCards[1] = 0;
                        }
                    }

                    if ((intDealerCards[0] != 0 & intDealerCards[1] != 0 & intDealerCards[2] != 0 & intDealerCards[3] != 0 & intDealerCards[4] != 0))
                    {
                        intDealerCards[2] = 0;
                        intDealerCards[3] = 0;
                        intDealerCards[4] = 0;
                    }
                } // END dblDealerHandTotal < 1

                if (dblDealerHandTotal < 2 & dblDealerHandTotal > 1) {
                    for (int i = 0; i <= 4; i++)
                    {
                        if (intDealerCards[1] / 10 != Math.Truncate((dblDealerHandTotal * 100) - 100))
                        {
                            intDealerCards[i] = 0;
                        }
                    }

                } // END dblDealerHandTotal < 2 & dblDealerHandTotal > 1

                if (dblDealerHandTotal < 3 & dblDealerHandTotal > 2)
                    for (int i = 0; i <= 4; i++)
                {
                    if (intDealerCards[i] / 1 == ((dblDealerHandTotal * 1000000) - (Math.Truncate((dblDealerHandTotal * 10000)) * 100)))
                    {
                        intDealerCards[i] = 0;
                    }
                } // END dblDealerHandTotal < 3 & dblDealerHandTotal > 1

            }
            if (dblDealerHandTotal < 4 & dblDealerHandTotal > 3)
            {
                for (int i = 0; i <= 4; i++)
                {
                    if (intDealerCards[1] / 10 != Math.Truncate((dblDealerHandTotal * 100) - 300))
                    {
                        intDealerCards[i] = 0;
                    }
                }
            } // END dblDealerHandTotal < 4 & dblDealerHandTotal > 3

          } // END dealerLogic
          
        // ******************* Main Form Load ***************************
        private void frmMain_Load(object sender, EventArgs e)
        {
            btnReplace.Enabled = false;
            chk1.Enabled = false;
            chk2.Enabled = false;
            chk3.Enabled = false;
            chk4.Enabled = false;
            chk5.Enabled = false;
            MinimizeBox = false;
            MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            lblWinner.Text = "No Game Started";
            scoresLabel.Text = "0.0 - High Card\n1.0 - One Pair\n2.0 - Two Pair\n3.0 - Three of A Kind\n4.0 - Straight\n5.0 - Flush\n6.0 - Full House\n7.0 - Four of A Kind\n8.0 - Straight Flush";

            // Code below is for Stats and bets
            btnOne.Enabled = false;
            btnFive.Enabled = false;
            btnTen.Enabled = false;
            btnTwentyFive.Enabled = false;
            btnFifty.Enabled = false;
            btnOneHundred.Enabled = false;
            btnFiveHundred.Enabled = false;
            lblJackpot.Text = dblJackpot.ToString("C2");
            lblPlayerWins.Text = (intPlayerWins.ToString() + "/10");
            lblPlayerLoses.Text = intPlayerLoses.ToString();
            lblPlayerRatio.Text = dblPlayerRatio.ToString("N2");
            lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            lblDealerWins.Text = (intDealerWins.ToString() + "/10");
            lblDealerLoses.Text = intDealerLoses.ToString();
            lblDealerRatio.Text = dblDealerRatio.ToString("N2");
            lblDealerWallet.Text = dblDealerWallet.ToString("C2");

        }

        //***********************************************************************************************
        // btnReplace - Replaces the players selected cards, as long as there are 3 or less to replace. *
        //***********************************************************************************************
        private void btnReplace_Click(object sender, EventArgs e)
        {
            double dblPlayerHandValue=0;
            double dblDealerHandValue =0;
            int intBoxTotal = 0;
            // Determine which checkboxes are checked to be replaced
            if ((chk1.Checked == true))
            {
                intBoxTotal = 1;
            }

            if ((chk2.Checked == true))
            {
                intBoxTotal++;
            }

            if ((chk3.Checked == true))
            {
                intBoxTotal++;
            }

            if ((chk4.Checked == true))
            {
                intBoxTotal++;
            }

            if ((chk5.Checked == true))
            {
                intBoxTotal++;
            }

            if ((intBoxTotal > 3))
            {
                MessageBox.Show("You\'re only able to replace up to 3 cards", "Replace Cards", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Replace cards if only there are no more than three cards selected
                if ((chk1.Checked == true))
                {
                    intPlayerCards[0] = this.Dealer(1);
                }

                if ((chk2.Checked == true))
                {
                    intPlayerCards[1] = this.Dealer(1);
                }

                if ((chk3.Checked == true))
                {
                    intPlayerCards[2] = this.Dealer(1);
                }

                if ((chk4.Checked == true))
                {
                    intPlayerCards[3] = this.Dealer(1);
                }

                if ((chk5.Checked == true))
                {
                    intPlayerCards[4] = this.Dealer(1);
                }

                // Uncheck checkboxes and hide them until its time to replace
                btnReplace.Enabled = false;
                btnOne.Enabled = false;
                btnFive.Enabled = false;
                btnTen.Enabled = false;
                btnTwentyFive.Enabled = false;
                btnFifty.Enabled = false;
                btnOneHundred.Enabled = false;
                btnFiveHundred.Enabled = false;
                chk1.Enabled = false;
                chk2.Enabled = false;
                chk3.Enabled = false;
                chk4.Enabled = false;
                chk5.Enabled = false;
                chk1.Checked = false;
                chk2.Checked = false;
                chk3.Checked = false;
                chk4.Checked = false;
                chk5.Checked = false;
                this.sort(ref intPlayerCards); // sort player cards after replacing
                // Display Player Cards after replacement
                picPC1.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intPlayerCards[0].ToString() + ".png")));
                picPC2.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intPlayerCards[1].ToString() + ".png")));
                picPC3.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intPlayerCards[2].ToString() + ".png")));
                picPC4.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intPlayerCards[3].ToString() + ".png")));
                picPC5.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intPlayerCards[4].ToString() + ".png")));
                dblPlayerHandValue = this.evaluateHand(ref intPlayerCards);

                // Call dealerLogic method
                this.dealerLogic(ref intDealerCards, dblDealerHandValue);
                // Dealer replaces cards
                for (int i = 0; (i <= 4); i++)
                {
                    if ((intDealerCards[i] == 0))
                    {
                        intDealerCards[i] = this.Dealer(1); // replace card
                    }

                }

                // Sort Dealer Hand After Replacing
                this.sort(ref intDealerCards);
                // Evaluate Dealer Hand
                dblDealerHandValue = this.evaluateHand(ref intDealerCards);
                // Display Dealer Cards After Replacements
                picDC1.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intDealerCards[0].ToString() + ".png")));
                picDC2.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intDealerCards[1].ToString() + ".png")));
                picDC3.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intDealerCards[2].ToString() + ".png")));
                picDC4.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intDealerCards[3].ToString() + ".png")));
                picDC5.ImageLocation = (Application.StartupPath + ("\\Resources\\"+ (intDealerCards[4].ToString() + ".png")));
            }
            // call dealerBets
            this.dealerBets();

            // Determine the Winner
            if ((dblPlayerHandValue > dblDealerHandValue))
            {
                // Player Wins
                lblWinner.Text = ("Congrats Player!"+ "\n" + "You\'ve won!"+ "\n" + "Dealer: "+ dblDealerHandValue.ToString()+ "\n" + "Player: " + dblPlayerHandValue.ToString());
                intPlayerWins = (intPlayerWins + 1);
                intDealerLoses = (intDealerLoses + 1);
                dblPlayerWallet = (dblPlayerWallet + dblJackpot);
                dblJackpot = 350;
                lblJackpot.Text = dblJackpot.ToString("C2");

                try
                {
                    dblPlayerRatio = (intPlayerWins / intPlayerLoses);
                    dblDealerRatio = (intDealerWins / intDealerLoses);
                }
                catch (DivideByZeroException)
                {
                    Console.WriteLine("Divide By Zero Exception");
                }
                lblPlayerWins.Text = (intPlayerWins.ToString() + "/10");
                lblPlayerLoses.Text = intPlayerLoses.ToString();
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
                lblDealerWins.Text = (intDealerWins.ToString() + "/10");
                lblDealerLoses.Text = intDealerLoses.ToString();
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");

                // End game after 10 wins
                if ((intPlayerWins == 10))
                {
                    DialogResult result=MessageBox.Show("Game over, you win!\nWould you like to play again?", "End", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        Application.Restart(); // restart application
                    }
                    else
                    {
                        this.Close(); // close the form
                    }
                }
            }
            else if ((dblPlayerHandValue == dblDealerHandValue))
            {
                // Tie
                lblWinner.Text = ("Contrats! We have a tie."+ "\n" + "Dealer: "+ dblDealerHandValue.ToString()+ "\n" + "Player: "+ dblPlayerHandValue.ToString() + "\n");
            }
            else
            {
                // Dealer Wins
                lblWinner.Text = ("Sorry Player\n"  + "You\'ve lost."+ "\n" + "Please Play Again!"+ "\n" + "Dealer: "+ dblDealerHandValue.ToString ()+ "\n" + "Player: "+ dblPlayerHandValue.ToString() + "\n");
                intDealerWins = (intDealerWins + 1);
                intPlayerLoses = (intPlayerLoses + 1);
                dblDealerWallet = (dblDealerWallet + dblJackpot);
                dblJackpot = 350;
                lblJackpot.Text = dblJackpot.ToString("C2");
                try
                {
                    dblPlayerRatio = (intPlayerWins / intPlayerLoses);
                    dblDealerRatio = (intDealerWins / intDealerLoses);
                }
                catch (DivideByZeroException)
                {
                    Console.WriteLine("Divide By Zero Exception");
                }
                lblPlayerWins.Text = (intPlayerWins.ToString() + "/10");
                lblPlayerLoses.Text = intPlayerLoses.ToString();
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
                lblDealerWins.Text = (intDealerWins.ToString() + "/10");
                lblDealerLoses.Text = intDealerLoses.ToString();
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
               
                // End game after 10 wins
                if ((intDealerWins == 10))
                {
                    DialogResult result=MessageBox.Show("Game over, you lose!\nWould you like to play again?", "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    
                    if (result == DialogResult.Yes) {
                        Application.Restart();
                    }
                    else
                    {
                        this.Close(); // close the form
                    }
                }      
            }

            // Calculate Win/Lose Ratio
            try
            {
                dblPlayerRatio = (intPlayerWins / intPlayerLoses);
                dblDealerRatio = (intDealerWins / intDealerLoses);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Divide By Zero Exception");
            }
            // Display Stats
            lblPlayerWins.Text = (intPlayerWins.ToString() + "/10");
            lblPlayerLoses.Text = intPlayerLoses.ToString();
            lblPlayerRatio.Text = dblPlayerRatio.ToString("N2");
            lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            lblDealerWins.Text = (intDealerWins.ToString() + "/10");
            lblDealerLoses.Text = intDealerLoses.ToString();
            lblDealerRatio.Text = dblDealerRatio.ToString("N2");
            lblDealerWallet.Text = dblDealerWallet.ToString("C2");
        }

      

        // ************* Dealer Bets Method *****************
        public void dealerBets()
        {
            // easy dealer betting code
            if((dblDealerWallet > 1500))
            {
                dblDealerWallet -= 650;
                dblJackpot += 650;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if((dblDealerWallet > 1200))
            {
                dblDealerWallet -= 700;
                dblJackpot += 650;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 1000))
            {
                dblDealerWallet -= 500;
                dblJackpot += 500;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if (((dblDealerWallet > 1000)
                        && (dblDealerRatio > 0.5)))
            {
                dblDealerWallet -= 400;
                dblJackpot += 400;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 750))
            {
                dblDealerWallet -= 550;
                dblJackpot += 550;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if (((dblDealerWallet > 650)
                        && (dblDealerRatio > 0.75)))
            {
                dblDealerWallet -= 400;
                dblJackpot += 400;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 650))
            {
                dblDealerWallet -= 150;
                dblJackpot += 150;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if (((intDealerWins > intPlayerWins)
                        && ((dblDealerWallet > 500)
                        && (dblDealerRatio > 0.5))))
            {
                dblDealerWallet -= 500;
                dblJackpot += 500;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 450))
            {
                dblDealerWallet -= 300;
                dblJackpot += 300;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if (((dblDealerWallet > 450)
                        && (dblDealerRatio > 1)))
            {
                dblDealerWallet -= 375;
                dblJackpot += 375;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 300))
            {
                dblDealerWallet -= 150;
                dblJackpot += 150;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 200))
            {
                dblDealerWallet -= 50;
                dblJackpot += 50;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 100))
            {
                dblDealerWallet -= 75;
                dblJackpot += 75;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }
            else if ((dblDealerWallet > 75))
            {
                dblDealerWallet -= 15;
                dblJackpot += 15;
                lblDealerWallet.Text = dblDealerWallet.ToString("C2");
                lblJackpot.Text = dblJackpot.ToString("C2");
            }

        }

        private void fileToolStripMenuItem1_DropDownOpened(object sender, EventArgs e)
        {
            fileToolStripMenuItem1.ForeColor = Color.Black;
        }

        private void fileToolStripMenuItem1_DropDownClosed(object sender, EventArgs e)
        {
            fileToolStripMenuItem1.ForeColor = Color.White;
        }
        // ************** Bet One Dollar - Click ********************
        private void btnOne_Click(object sender, EventArgs e)
        {
            // Bet 1 Dollar
            if ((dblPlayerWallet >= 1))
            {
                dblPlayerWallet--;
                dblJackpot++;
                // Display Jackpot Amount and Player Wallet Remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Empty Wallet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
        }
        // ************* Bet Five Dollars - Click ******************
        private void btnFive_Click(object sender, EventArgs e)
        {
            //Bet 5 Dollars
            if ((dblPlayerWallet >= 5))
            {
                dblPlayerWallet -= 5;
                dblJackpot += 5;
                // Display Jackpot Amount and Player Wallet Remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Empty Wallet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // ************* Bet Ten Dollars - Click ******************
        private void btnTen_Click(object sender, EventArgs e)
        {
            //Bet 10 Dollars
            if ((dblPlayerWallet >= 10))
            {
                dblPlayerWallet -= 10;
                dblJackpot += 10;
                // Display Jackpot Amount and Player Wallet Remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Empty Wallet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // ************* Bet Twenty Five Dollars - Click ******************
        private void btnTwentyFive_Click(object sender, EventArgs e)
        {
            // Bet 25 Dollars
            if ((dblPlayerWallet >= 25))
            {
                dblPlayerWallet -= 25;
                dblJackpot += 25;
                // Display Jackpot Amount and Player Wallet Remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        // ************* Bet Fifty Dollars - Click ******************
        private void btnFifty_Click(object sender, EventArgs e)
        {
            // Bet 50 Dollars
            if ((dblPlayerWallet >= 50))
            {
                dblPlayerWallet -= 50;
                dblJackpot += 50;
                // Display Jackpot Amount and Player Wallet Remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ************* Bet One Hundred Dollars - Click ******************
        private void btnOneHundred_Click(object sender, EventArgs e)
        {
            // Bet 100 Dollars
            if ((dblPlayerWallet >= 100))
            {
                dblPlayerWallet -= 100;
                dblJackpot += 100;
                // Display Jackpot Amount and Player Wallet Remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // ************* Bet Five Hundred Dollars - Click ******************
        private void btnFiveHundred_Click(object sender, EventArgs e)
        {
            // Bet 500 Dollars
            if((dblPlayerWallet >= 500))
            {
                dblPlayerWallet -= 500;
                dblJackpot += 500;
                // display jackpot amount and player wallet remainings
                lblJackpot.Text = ("$" + dblJackpot.ToString("N2"));
                lblPlayerWallet.Text = dblPlayerWallet.ToString("C2");
            }
            else
            {
                MessageBox.Show("You dont have enough money!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    } // End frmMain
}
