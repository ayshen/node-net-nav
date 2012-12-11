using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using Multitouch.Framework.WPF.Input;


namespace NodeNetworkNavigator
{
    // <summary>Dumb wrapper class</summary>
    public class Node
    {
        // <summary>Everything to do with MainWindow</summary>
        public EditWindow editWindow { get; set; }

        //managing the appearance on the app
        public class DrawingConfiguration
        {
            //the node
            public System.Windows.Shapes.Ellipse ellipse { get; set; }
            public System.Windows.Controls.TextBlock name { get; set; }
            public System.Windows.Controls.Grid mainGrid { get; set; }
            public bool selected { get; set; }
            public int fingers { get; set; }

            public ArrayList connectionLines { get; set; }
            // more things here
            public DrawingConfiguration(System.Windows.Controls.Grid mainGrid)
            {
                this.mainGrid = mainGrid;
                this.connectionLines = new ArrayList();
            }

            // also methods
            public void setLines(List<int> targets)
            {
            }

            public void setCircle(Point topleft, int num)
            {
                ellipse = new Ellipse();
                ellipse.VerticalAlignment = VerticalAlignment.Top;
                ellipse.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                ellipse.Height = 300;
                ellipse.Width = 300;
                ellipse.Margin = new Thickness(topleft.X, topleft.Y, 0, 0);
                ellipse.Fill = Brushes.White;
                ellipse.Opacity = 150;
                mainGrid.Children.Add(ellipse);

                
                //now for the text
                name = new TextBlock();
                name.Text = "Initial Text " + num;
                name.Margin = new Thickness(topleft.X + 15, topleft.Y + 100, 0, 0);
                name.Foreground = Brushes.Red;
                name.FontSize = 50d;
                name.VerticalAlignment = VerticalAlignment.Top;
                name.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                name.MaxWidth = 250;
                name.TextWrapping = TextWrapping.WrapWithOverflow;
                name.Height = 80;
                //name.Height = 80;
                mainGrid.Children.Add(name);
  
                
            }

            public void createConnection(Node node, Point center) {
                Line connection = new Line();
                connection.Stroke = Brushes.SpringGreen;
                connection.StrokeThickness = 30;
                connection.VerticalAlignment = VerticalAlignment.Top;
                connection.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                connection.Y1 = ellipse.Height/2 + center.Y;
                connection.Y2 = node.drawingConfiguration.ellipse.Height/2 + node.data.topleft.Y;
                connection.X1 = ellipse.Width/2 + center.X;
                connection.X2 = node.drawingConfiguration.ellipse.Width/ 2 + node.data.topleft.X;

                   /* connection.Y1 = center.Y + 600 + ellipse.Height;
                    connection.Y2 = node.data.center.Y + 600 + node.drawingConfiguration.ellipse.Height/2;
         
                        connection.X1 = center.X + 800 + ellipse.Width/2;
                        connection.X2 = node.data.center.X + 800 + node.drawingConfiguration.ellipse.Width/2;
                */
                mainGrid.Children.Add(connection);
                //MultitouchScreen.AddContactMovedHandler(connection, connection_Drag);
                connectionLines.Add(connection);
            }
        }

        // <summary>Base class for info.</summary>
        public class Data
        {
            public System.Int32 id { get; set; }
            public System.String name { get; set; }
            public System.Collections.Generic.List<System.Int32> connectionTargets { get; set; }
            public Point topleft { get; set; }
            public int number { get; set; }
            // TODO
            public Data(Point center, int number)
            {
                this.topleft = center;
                this.number = number;
                connectionTargets = new System.Collections.Generic.List<System.Int32>();
            }
        }

        // <summary>Text info.</summary>
       /* public abstract class TextData : 5
        {
            public System.String text;
        }*/

        // <summary> instances of these classes </summary>
        protected Node.DrawingConfiguration drawingConfiguration;
        public Node.Data data;
        public MainWindow window { get; set; }

        public Node(System.Windows.Controls.Grid mainGrid, Point center, MainWindow window, int number)
        {
              //this.thing or relate between each other
            drawingConfiguration = new DrawingConfiguration(mainGrid);
            data = new Node.Data(center, number);
            this.window = window;
            //anything we need to initially change about the drawingConfiguration
            drawingConfiguration.setLines(data.connectionTargets);
            drawingConfiguration.setCircle(data.topleft, number);
            MultitouchScreen.AddContactEnterHandler(drawingConfiguration.ellipse, finger_Contact);
            MultitouchScreen.AddContactRemovedHandler(drawingConfiguration.ellipse, finger_Lift);
            MultitouchScreen.AddContactLeaveHandler(drawingConfiguration.ellipse, finger_Lift);
            MultitouchScreen.AddContactMovedHandler(drawingConfiguration.ellipse, finger_Drag);

            MultitouchScreen.AddContactEnterHandler(drawingConfiguration.name, finger_Contact);
            MultitouchScreen.AddContactRemovedHandler(drawingConfiguration.name, finger_Lift);
            MultitouchScreen.AddContactLeaveHandler(drawingConfiguration.name, finger_Lift);
            MultitouchScreen.AddContactMovedHandler(drawingConfiguration.name, finger_Drag);
        }

        /// <summary>
        /// When the ellipse is touched, we show that it's being selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void finger_Contact(object sender, ContactEventArgs e)
        {
            drawingConfiguration.selected = true;
            drawingConfiguration.ellipse.Fill = Brushes.Green;
            
            Int32 state = window.getAppState();
            if (state == MainWindow.APPSTATE_CHOOSE_FIRST_NODE_TO_TOGGLE_CONNECTION || state == MainWindow.APPSTATE_CHOOSE_SECOND_NODE_TO_TOGGLE_CONNECTION)
            {
                window.makeConnection(data.number);
            }
            else if (state == MainWindow.APPSTATE_CHOOSE_NODE_TO_DELETE)
            {
                this.delete();
            }
            else if (state == MainWindow.APPSTATE_CHOOSE_NODE_TO_EDIT)
            {
                window.editNode(this);
            } else if (state == MainWindow.APPSTATE_DEFAULT) {
                Point position = e.GetPosition(drawingConfiguration.ellipse);
                this.moveThis(position);
            }
            //Point pos = e.GetPosition(null);
        }

        /// <summary>
        /// When another node moves, this moves the end of the line that points from this node to the moved one
        /// </summary>
        /// <param name="pos">the amount the end of a line should move</param>
        /// <param name="n">the node that should move</param>
        public void moveEndLine(Point pos, int n)
        {
            int lineNum = data.connectionTargets.IndexOf(n);
            Line conn = (Line) drawingConfiguration.connectionLines[lineNum];
            conn.X2 = conn.X2 + (pos.X) - drawingConfiguration.ellipse.Width/2;
            conn.Y2 = conn.Y2 + (pos.Y) - drawingConfiguration.ellipse.Height/2;
        }

        /// <summary>
        /// delete this current node
        /// </summary>
        private void delete()
        {
            drawingConfiguration.mainGrid.Children.Remove(drawingConfiguration.ellipse);
            drawingConfiguration.mainGrid.Children.Remove(drawingConfiguration.name);
            for (int i = 0; i < data.connectionTargets.Count; i++)
            {
                int connectionNum = data.connectionTargets[i];
                window.deleteConnection(connectionNum, data.number);
                drawingConfiguration.mainGrid.Children.Remove((Line)drawingConfiguration.connectionLines[i]);
            }
        }

        /// <summary>
        /// Move this node because we've dragged it
        /// </summary>
        /// <param name="pos">the amount we moved it</param>
        private void moveThis(Point pos)
        {
            //change line positions
            foreach (Object conn in drawingConfiguration.connectionLines)
            {
                ((Line)conn).X1 = drawingConfiguration.ellipse.Margin.Left + (pos.X);
                ((Line)conn).Y1 = drawingConfiguration.ellipse.Margin.Top + (pos.Y);
            }

            Point passedPos = new Point(drawingConfiguration.ellipse.Margin.Left + (pos.X),
                drawingConfiguration.ellipse.Margin.Top + (pos.Y));

            foreach (int n in data.connectionTargets)
            {
                window.moveLine(n, pos, data.number);
            }

            //change the text position
            drawingConfiguration.name.Margin = new Thickness(
                drawingConfiguration.name.Margin.Left + (pos.X - drawingConfiguration.ellipse.Width / 2),
                drawingConfiguration.name.Margin.Top + (pos.Y - drawingConfiguration.ellipse.Height / 2),
                0, 0);

            //change the node
            drawingConfiguration.ellipse.Margin = new Thickness(
                drawingConfiguration.ellipse.Margin.Left + (pos.X - drawingConfiguration.ellipse.Width / 2),
                drawingConfiguration.ellipse.Margin.Top + (pos.Y - drawingConfiguration.ellipse.Height / 2),
                0, 0);

            data.topleft = new Point(data.topleft.X + (pos.X - drawingConfiguration.ellipse.Width / 2),
                                    data.topleft.Y + (pos.Y - drawingConfiguration.ellipse.Height / 2));

        }

        /// <summary>
        /// when we drag a node we either want to delete it or move it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void finger_Drag(object sender, ContactEventArgs e)
        {
            Point pos = e.GetPosition(drawingConfiguration.ellipse);
            Point currpos = new Point(drawingConfiguration.ellipse.Margin.Left, drawingConfiguration.ellipse.Margin.Top);
            bool nearLeft = currpos.X + pos.X - drawingConfiguration.ellipse.Width / 2 < 0;
            bool nearTop = currpos.Y + pos.Y - drawingConfiguration.ellipse.Height / 2 < 0;

            Int32 state = window.getAppState();
            if (state == MainWindow.APPSTATE_DEFAULT)
            {
                moveThis(pos);
            }
            //if it's within the edge, then delete it
        }

        protected void finger_Lift(object sender, ContactEventArgs e)
        {
            drawingConfiguration.selected = false;
            drawingConfiguration.ellipse.Fill = Brushes.White;
            //editWindow.Show();
        }

        /// <summary>
        /// create a connection between this node and the one passed in
        /// </summary>
        /// <param name="nodeNumber">the ID of the node</param>
        /// <param name="node">the node to make a connection with</param>
        public void createConnection(int nodeNumber, Node node)
        {
            if (!data.connectionTargets.Contains(nodeNumber))
            {
                drawingConfiguration.createConnection(node, data.topleft);
                data.connectionTargets.Add(nodeNumber);
            }
        }

        /// <summary>
        /// remove a connection with the node at nodeNumber
        /// </summary>
        /// <param name="nodeNumber"></param>
        public void removeConnection(int nodeNumber)
        {
            int connection = data.connectionTargets.IndexOf(nodeNumber);
            Line l = (Line)drawingConfiguration.connectionLines[connection];
            drawingConfiguration.mainGrid.Children.Remove(l);
            drawingConfiguration.connectionLines.Remove(l);
            data.connectionTargets.Remove(nodeNumber);
        }
        public Boolean getSelected()
        {
            return drawingConfiguration.selected;
        }

        public void redrawCircle()
        {
            drawingConfiguration.mainGrid.Children.Remove(drawingConfiguration.ellipse);
            drawingConfiguration.mainGrid.Children.Add(drawingConfiguration.ellipse);

            drawingConfiguration.mainGrid.Children.Remove(drawingConfiguration.name);
            drawingConfiguration.mainGrid.Children.Add(drawingConfiguration.name);
        }

        public void changeContent(String title)
        {
            drawingConfiguration.name.Text = title;
            data.name = title;
            //String newContent = Interaction.InputBox("hi", "hellow", "nothing", 10, 10);
            //make this into a keyboard input which activates the correct function

        }

        private void addToContent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            drawingConfiguration.name.Text = drawingConfiguration.name.Text + "a";
            data.name = drawingConfiguration.name.Text;
        }
    }
}
