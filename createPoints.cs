using UnityEngine;
using UnityEngine.UI; // For displaying things on the screen
using System.Collections;
using System.IO; // For the dynamic input of files

public class createPoints : MonoBehaviour {
	
	public TextAsset cerveau; // Initial data
	public Material currentMaterial; // Material to use to create the meshes
	public Text display; // Used for testing
	
	private int nbVertices; // Number of vertices
	private int nbImages; // Number of map of potentials provided during the run
	private int numLayer;
	private float d=0.2f; // Scale of the spheres
	private GameObject[] go;
	private int sleep; // Frequency of changing the data.

	void Start () { // Initialization with the data in the ressource file
		
		//display.text = "test";
		
		// Creation of a directory to store the data obtained, only for windows
		#if UNITY_STANDALONE_WIN
		FileInfo theSourceFile = null; // File to get
		StreamReader reader = null; // Stream to read from the file
		string subDirectory="test";
		
		theSourceFile = new FileInfo (Application.dataPath + "/configuration.txt"); // Getting the file
		if ( theSourceFile != null && theSourceFile.Exists ) {
			reader = theSourceFile.OpenText(); // Getting the stream
			string[] configuration = (reader.ReadToEnd ()).Split('\n'); // Reading the configuration
			subDirectory=configuration[0];
		}			
		#endif
		
		nbImages=1;
		int day = System.DateTime.Now.Day;
		string pathString = System.IO.Path.Combine(Application.dataPath, subDirectory);
		
		// Creation of the mesh

		
		// Parsing data from the mesh file
		string input = cerveau.text;
		string[] lines = input.Split ('\n'); // For separating the lines

		int nb = 6088; // number of octahedras inside the brain (413984=6088*68)

		go = new GameObject[68]; // number of different GamoObject used (one can't have more than 65000 vertices)

		for (int j=0; j<68; j++) { // Creation of all the 68 objects

			go[j]=new GameObject(); // Creation of the j-th GameObject
			MeshFilter mf=go[j].AddComponent<MeshFilter>() as MeshFilter; // Attribution of a MeshFilter
			MeshRenderer mr=go[j].AddComponent<MeshRenderer>() as MeshRenderer; // And a MeshRnderer
			mr.material=currentMaterial; // Assignation of the material (we use a specific material with whiwh we can plot different colors inside a same mesh)
			Mesh mesh = new Mesh (); // Creation o the mesh
			mf.mesh = mesh; // Attribution of the mesh in the MeshFilter

			int nbTriangles=3*8*2; // The number of triangles to create equals 3 (each triangles has 3 vertices) * 8 (an octahedra has 8 triangles) * 2 (Each triangle is created 2 times with opposite normals)
			int edges=6; // An octahedra has 6 vertices
			Vector3[] vertices = new Vector3[nb * edges]; // Creation of the list of vertices
			Color[] colors = new Color[nb * edges]; // Creation of the colors list
			int[] triangles = new int[nbTriangles * nb]; // Creation of the triangles list
			for (int i=0; i<nb; i++) { // Creation of the nb=6088 octahedras
				string[] line_i = lines [i + nb * j].Split (' '); // Parsing of the i-th line of the data

				// Getting the 3 positionnal space values and the activity value
				float a = float.Parse (line_i [1].Substring (1, line_i[1].Length - 2));
				float c = float.Parse (line_i [2].Substring (0, line_i[2].Length - 1));
				float b = float.Parse (line_i [3].Substring (0, line_i[3].Length - 1));
				float val = float.Parse (line_i [4].Substring (1, line_i[4].Length - 3));

				// Attribution of a size for the octahedras, which is bigger if the activity value is high
				if (val<=0.4) d=0.1f;
				else d=0.3f;

				// Creation of the 6 vertices of the octahedra
				vertices [edges * i + 0] = new Vector3 (a+d, b, c);
				vertices [edges * i + 1] = new Vector3 (a-d, b, c);
				vertices [edges * i + 2] = new Vector3 (a, b-d, c+d);
				vertices [edges * i + 3] = new Vector3 (a, b+d, c+d);
				vertices [edges * i + 4] = new Vector3 (a, b+d, c-d);
				vertices [edges * i + 5] = new Vector3 (a, b-d, c-d);

				// Creation of all the 16 triangles (so 48 values to enter in the triangles list)
				triangles [nbTriangles * i + 0] = edges * i + 0;
				triangles [nbTriangles * i + 1] = edges * i + 2;
				triangles [nbTriangles * i + 2] = edges * i + 3;

				triangles [nbTriangles * i + 3] = edges * i + 0;
				triangles [nbTriangles * i + 4] = edges * i + 3;
				triangles [nbTriangles * i + 5] = edges * i + 4;

				triangles [nbTriangles * i + 6] = edges * i + 0;
				triangles [nbTriangles * i + 7] = edges * i + 4;
				triangles [nbTriangles * i + 8] = edges * i + 5;

				triangles [nbTriangles * i + 9] = edges * i + 0;
				triangles [nbTriangles * i + 10] = edges * i +5;
				triangles [nbTriangles * i + 11] = edges * i +2;

				triangles [nbTriangles * i + 12] = edges * i + 1;
				triangles [nbTriangles * i + 13] = edges * i + 2;
				triangles [nbTriangles * i + 14] = edges * i + 3;

				triangles [nbTriangles * i + 15] = edges * i + 1;
				triangles [nbTriangles * i + 16] = edges * i + 3;
				triangles [nbTriangles * i + 17] = edges * i + 4;

				triangles [nbTriangles * i + 18] = edges * i + 1;
				triangles [nbTriangles * i + 19] = edges * i + 4;
				triangles [nbTriangles * i + 20] = edges * i + 5;

				triangles [nbTriangles * i + 21] = edges * i + 1;
				triangles [nbTriangles * i + 22] = edges * i + 5;
				triangles [nbTriangles * i + 23] = edges * i + 2;

				// The 8 last triangles are created in function of the 8 first
				int x=24;
				for (int k=0;k<8;k++) {
					triangles[nbTriangles * i+x+3*k]=triangles[nbTriangles * i+3*k];
					triangles[nbTriangles * i+x+3*k+1]=triangles[nbTriangles * i+3*k+2];
					triangles[nbTriangles * i+x+3*k+2]=triangles[nbTriangles * i+3*k+1];
				}

				// Attribution of the colors and transparency of the vertices of the octahedras, in function of the activity value
				for (int k=0;k<edges;k++) {
					if ((0.0<val)&&(val<=0.05)) colors[edges*i+k]=new Vector4(0f,0f,0.0f,0f);
					if ((0.05<val)&&(val<=0.1)) colors[edges*i+k]=new Vector4(0f,0f,0.0f,0.01f);
					if ((0.1<val)&&(val<=0.15)) colors[edges*i+k]=new Vector4(0f,0f,0.0f,0.02f);
					if ((0.15<val)&&(val<=0.2)) colors[edges*i+k]=new Vector4(0f,0f,0.0f,0.03f);
					if ((0.2<val)&&(val<=0.25)) colors[edges*i+k]=new Vector4(0f,0f,0.0f,0.4f);
					if ((0.25<val)&&(val<=0.3)) colors[edges*i+k]=new Vector4(1f,0.0f,0.0f,0.5f);
					if ((0.3<val)&&(val<=0.35)) colors[edges*i+k]=new Vector4(255f,0f,0.0f,0.5f);
					if ((0.35<val)&&(val<=0.4)) colors[edges*i+k]=new Vector4(255f,0f,0.0f,1f);
					if ((0.4<val)&&(val<=0.45)) colors[edges*i+k]=new Vector4(255f,255f,0f,1f);
					if ((0.45<val)&&(val<=0.5)) colors[edges*i+k]=new Vector4(255f,255f,255f,1f);
				}
			}
					
			// Assigning the vertices, triangles and colors to the mesh
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.colors = colors;
		}
		//Initialization of the animation of colors changing
		sleep = 0;
		display.text = sleep.ToString ();
	}

	void Update() {
		// Updating the colors from a file in the Data sublocation of the executable
		// This only works on windows
		/*
		#if UNITY_STANDALONE_WIN
		FileInfo theSourceFile = null; // File to get
		StreamReader reader = null; // Stream to read from the file
		
		theSourceFile = new FileInfo (Application.dataPath + "/Sl_Potential_Pyramid.txt"); // Getting the file
		if ( theSourceFile != null && theSourceFile.Exists ) {
			reader = theSourceFile.OpenText(); // Getting the stream
			potentials = (reader.ReadToEnd ()).Split('\n'); // Reading the potentials
			colors = mesh.colors; // Getting the actual color list
			colorBrain (); // Updating the colors
			mesh.colors = colors; // Updating the mesh
		}
		#endif*/
		if (sleep < 20) {
			sleep += 1;
			display.text = sleep.ToString ();
		} 
		else {
			for (int j=0; j<68; j++) {
				MeshFilter mf = go [j].GetComponent<MeshFilter> ();
				Mesh mesh = mf.mesh;
				Color[] colors = mesh.colors;
				for (int i=0; i<colors.Length; i++) {
					float r = colors [i].r;
					float g = colors [i].g;
					float b = colors [i].b;
					float a = colors [i].a;
					r += Random.Range (-0.5f, 0.5f);
					g += Random.Range (-0.5f, 0.5f);
					b += Random.Range (-0.5f, 0.5f);
					colors [i] = new Vector4 (r, g, b, a);
				}
				mesh.colors = colors;
			}
			sleep=0;
			display.text = sleep.ToString ();
		}
	}
}
