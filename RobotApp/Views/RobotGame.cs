// Copyright (c) Converter Systems LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Urho;

namespace RobotApp.Views
{
    public class RobotGame : Application
    {
        private Scene scene;
        private Node cameraNode;
        private Node axis1Node;
        private Node axis2Node;
        private Node axis3Node;
        private Node axis4Node;

        static RobotGame()
        {
            UnhandledException += (s, e) => { Debug.WriteLine(e.Exception.Message); e.Handled = true; };
        }

        public RobotGame(ApplicationOptions options = null)
            : base(options)
        {
        }

        protected override void Start()
        {
            base.Start();
            this.CreateScene();
            this.SetupViewport();
        }

        protected override void Stop()
        {
            base.Stop();
            this.scene?.Dispose();
            this.scene = null;
            this.cameraNode = null;
            this.axis1Node = null;
            this.axis2Node = null;
            this.axis3Node = null;
            this.axis4Node = null;
        }

        private void CreateScene()
        {
            this.scene = new Scene();

            // Load scene content prepared in the editor (XML format).
            this.scene.LoadXmlFromCache(this.ResourceCache, "Scenes/irb6700_scene.xml");

            this.axis1Node = this.scene.GetChild("Link1", true);
            this.axis2Node = this.scene.GetChild("Link2", true);
            this.axis3Node = this.scene.GetChild("Link3", true);
            this.axis4Node = this.scene.GetChild("Link4", true);

            // Create the camera (not included in the scene file)
            this.cameraNode = this.scene.CreateChild("Camera");
            this.cameraNode.CreateComponent<Camera>();

            // Set an initial position for the camera scene node above the plane
            this.cameraNode.Position = new Vector3(0.0f, 1.2f, -6.0f);

            // Don't need 200 fps for a visualization
            this.Engine.MinFps = 4;
            this.Engine.MaxFps = 12;
        }

        private void SetupViewport()
        {
            this.Renderer.SetViewport(0, new Viewport(this.Context, this.scene, this.cameraNode.GetComponent<Camera>(), null));
        }

        private float axis1;

        public float Axis1
        {
            get { return this.axis1; }

            set
            {
                if (this.axis1Node != null && this.axis1 != value)
                {
                    this.axis1 = value;
                    InvokeOnMain(() => this.axis1Node.Rotation = new Quaternion(0, -this.axis1, 0));
                }
            }
        }

        private readonly Vector3 axis2axis = new Vector3(0f, 0.790f, 0.31f);
        private float axis2;

        public float Axis2
        {
            get { return this.axis2; }

            set
            {
                if (this.axis2Node != null && this.axis2 != value)
                {
                    var delta = value - this.axis2;
                    this.axis2 = value;
                    InvokeOnMain(() => this.axis2Node.RotateAround(this.axis2axis, Quaternion.FromAxisAngle(Vector3.Left, delta), TransformSpace.Local));
                }
            }
        }

        private readonly Vector3 axis3axis = new Vector3(0f, 1.9f, 0.32f);
        private float axis3;

        public float Axis3
        {
            get { return this.axis3; }

            set
            {
                if (this.axis3Node != null && this.axis3 != value)
                {
                    var delta = value - this.axis3;
                    this.axis3 = value;
                    InvokeOnMain(() => this.axis3Node.RotateAround(this.axis3axis, Quaternion.FromAxisAngle(Vector3.Left, delta), TransformSpace.Local));
                }
            }
        }

        private readonly Vector3 axis4axis = new Vector3(0f, 2.1f, 0.344f);
        private float axis4;

        public float Axis4
        {
            get { return this.axis4; }

            set
            {
                if (this.axis4Node != null && this.axis4 != value)
                {
                    var delta = value - this.axis4;
                    this.axis4 = value;
                    InvokeOnMain(() => this.axis4Node.RotateAround(this.axis4axis, Quaternion.FromAxisAngle(Vector3.UnitZ, delta), TransformSpace.Local));
                }
            }
        }
    }
}
