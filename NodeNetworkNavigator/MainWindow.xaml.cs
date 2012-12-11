using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Multitouch.Framework.WPF.Input;
using InteractiveSpace.SDK;
using InteractiveSpace.SDK.DLL;
using Microsoft.VisualBasic;

namespace NodeNetworkNavigator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow : Multitouch.Framework.WPF.Controls.Window
    {
        public static Int32 APPSTATE_DEFAULT = 0;
        public static Int32 APPSTATE_CHOOSE_NODE_TO_EDIT = 1;
        public static Int32 APPSTATE_CHOOSE_FIRST_NODE_TO_TOGGLE_CONNECTION = 11;
        public static Int32 APPSTATE_CHOOSE_SECOND_NODE_TO_TOGGLE_CONNECTION = 12;
        public static Int32 APPSTATE_CHOOSE_NODE_TO_DELETE = 2;

        private Int32 appState = APPSTATE_DEFAULT;


        InteractiveSpaceProvider spaceProvider;
        ArrayList nodes;
        EditWindow editWindow;
        TextBlock signal;

        public MainWindow()
        {
            InitializeComponent();

            MultitouchScreen.AllowNonContactEvents = true;
            
            spaceProvider = new InteractiveSpaceProviderDLL();
            spaceProvider.Connect();

            nodes = new ArrayList(5);

            editWindow = new EditWindow();

            //Uncomment this line to enable raw video streaming.
            /*
            spaceProvider.CreateRawVideoStreaming();
            */

            //Uncomment these lines to draw fingers on the projected screen
            //*
            spaceProvider.CreateFingerTracker();
            vizLayer.SpaceProvider = spaceProvider;
            //*/
            signal = new TextBlock();
            signal.FontSize = 50;
            signal.Text = "0";
            signal.Height = 80;
            signal.MaxWidth = 80;
            signal.Foreground = Brushes.White;
            signal.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            signal.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            signal.Margin = new Thickness(0, 0, 0, 0);
           // mainGrid.Children.Add(signal);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            spaceProvider.Close();
        }

        public Int32 getAppState()
        {
            return this.appState;
        }

        private void changeAppStateTo(Int32 state)
        {
            this.appState = state;
            signal.Text = state.ToString();
        }

        public void createButton_Click(object sender, ContactEventArgs e)
        {
            //if (this.appState != APPSTATE_DEFAULT) return;

            /*
                        Int32 id = this.nodes.Keys.Count;
                        Node n = new Node(MainGrid, new System.Windows.Point(600, 600),
                                this, id);
                        this.nodes.Add(id, n);
            */
            Node created = new Node(mainGrid, new Point(300, 300), this, nodes.Count);
            nodes.Add(created);
/*            Node n = Node.ForWPFContext(this, MainGrid);
            this.nodes.Add(n.data.id, n);*/
        }

        public void editButton_Click(object sender, ContactEventArgs e)
        {
            if (this.appState == APPSTATE_CHOOSE_NODE_TO_EDIT)
            {
                this.changeAppStateTo(APPSTATE_DEFAULT);
                //get rid of the popup here
                return;
            }

            //to me this is disabling potential gestures since we're changing the state anyways. are there interum states we aren't covering?
            /*if (this.appState != APPSTATE_DEFAULT)
                return;
            */
            this.changeAppStateTo(APPSTATE_CHOOSE_NODE_TO_EDIT);
        }

        public void connectionButton_Click(object sender, ContactEventArgs e)
        {
            if (this.appState == APPSTATE_CHOOSE_FIRST_NODE_TO_TOGGLE_CONNECTION ||
                    this.appState == APPSTATE_CHOOSE_SECOND_NODE_TO_TOGGLE_CONNECTION)
            {
                this.changeAppStateTo(APPSTATE_DEFAULT);
                //get rid of the popup here
                return;
            }

            //if (this.appState != APPSTATE_DEFAULT)
            //    return;

            this.changeAppStateTo(APPSTATE_CHOOSE_FIRST_NODE_TO_TOGGLE_CONNECTION);
        }

        public void trashButton_Click(object sender, ContactEventArgs e)
        {
            if (this.appState == APPSTATE_CHOOSE_NODE_TO_DELETE)
            {
                this.changeAppStateTo(APPSTATE_DEFAULT);
                //get rid of the popup here
                return;
            }
            /*
            if (this.appState != APPSTATE_DEFAULT)
                return;*/

            this.changeAppStateTo(APPSTATE_CHOOSE_NODE_TO_DELETE);
        }

        public void makeConnection(int node1)
        {
            Node caller = (Node)nodes[node1];
            for (int n = 0; n < nodes.Count; n++)
            {
                if (((Node)nodes[n]).getSelected() && (n != node1))
                {
                    ((Node)nodes[n]).createConnection(node1, caller);
                    caller.createConnection(n, ((Node)nodes[n]));
                    ((Node)nodes[n]).redrawCircle();
                    caller.redrawCircle();
                    n = nodes.Count;
                }
            }
        }

        public void moveLine(int node, Point pos, int requester)
        {
            Node nodething = (Node)nodes[node];
            nodething.moveEndLine(pos, requester);
        }

        public void deleteConnection(int nodeThatsStaying, int otherNode)
        {
            Node staying = (Node)nodes[nodeThatsStaying];
            staying.removeConnection(otherNode);
        }

        /// <summary>
        /// When we select the edit option, this makes the current edit window into a new one
        /// </summary>
        /// <param name="node"></param>
        public void editNode(Node node)
        {
            //1. Destroy the previous one.
            this.editWindow.Hide();
            this.editWindow = null;
            //2. Create a new one with this node.
            this.editWindow = new EditWindow(node.data.name, node);
            this.editWindow.Show();
        }
    }
}
