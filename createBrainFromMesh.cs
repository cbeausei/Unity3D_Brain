using UnityEngine;
using UnityEngine.UI; // For displaying things on the screen
using System.Collections;
using System.IO; // For the dynamic input of files

public class createBrainFromMesh : MonoBehaviour {
	
	public TextAsset cerveau; // Data from Cerveau.obj (list of vertices and triangles)
	public TextAsset data; // Data of potentials of vertices
	public Text display; // Display Text used for testing
	public int layer;
	public string color;
	
	private string s; // Used to read each line of potentials
	private float f; // Value of current potential
	private Mesh mesh; // Mesh of the brain
	private int nbVertices; // Number of vertices
	private string[] potentials; // List of potentials
	private Color[] colors; // List of colors of the Mesh corresponding to the vertices
	private int nbImages; // Number of map of potentials provided during the run
	private int numLayer;
	
	void Start () { // Initialization with the data in the ressource file
		
		display.text = "test";

		// Creation of a directory to store the data obtained
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
		MeshFilter mf = GetComponent<MeshFilter> ();
		mesh = new Mesh ();
		mf.mesh = mesh;
		
		// Parsing data from the mesh file
		string input = cerveau.text;
		string[] lines = input.Split ('\n'); // For separating the lines
		string[] line4 = lines [4].Split (' '); // Getting the first line
		string[] line151166 = lines [151166].Split (' '); // Getting the second line
		string[] line392372 = lines [392372].Split (' ');
		int indexStartTriangles = 151167;
		nbVertices = int.Parse(line4 [1]);
		int nbTriangles = int.Parse (line151166 [1]);
		int nbTetrahedra = int.Parse (line392372 [1]);

		int nbVerticesInCurrentLayer = 0;
		int[] layerOfVertex = new int[nbVertices];
		for (int i=0; i<nbVertices; i++) {
			string[] line_i=lines[i+5].Split (' ');
			int layerTemp=int.Parse (line_i[line_i.Length-1]);
			layerOfVertex[i]=layerTemp;
			if (layerTemp==layer) nbVerticesInCurrentLayer++;
		}

		// Getting the vertices from the file
		Vector3[] vertices=new Vector3[nbVerticesInCurrentLayer];
		int[] localization=new int[nbVertices];
		int currentIndex = 0;
		for (int i=0; i<nbVertices; i++) {
			string[] line_i = lines [i + 5].Split (' ');
			if (layerOfVertex [i] == layer) {
				int a = 1;
				while (line_i[a].Length==0)
					a++;
				int c = a + 1;
				while (line_i[c].Length==0)
					c++;
				int b = c + 1;
				while (line_i[b].Length==0)
					b++;
				vertices [currentIndex] = new Vector3 (float.Parse (line_i [a]), float.Parse (line_i [b]), float.Parse (line_i [c]));
				localization[i]=currentIndex;
				currentIndex++;
			}
		}

		// Getting the triangles from the file
		int nbTrianglesInCurrentLayer = 0;
		for (int i=0; i<nbTriangles; i++) {
			string[] line_i=lines[i+indexStartTriangles].Split (' ');
			int a=int.Parse(line_i[1])-1;
			if (layerOfVertex[a]==layer) nbTrianglesInCurrentLayer++;
		}

		currentIndex = 0;
		int[] triangles = new int[3*nbTrianglesInCurrentLayer];
		for (int i=0; i<nbTriangles; i++) {
			string[] line_i=lines[i+indexStartTriangles].Split (' ');
			int x=int.Parse(line_i[1])-1;
			if (layerOfVertex[x]==layer) {
				int a=int.Parse (line_i[1])-1;
				int b=int.Parse (line_i[2])-1;
				int c=int.Parse (line_i[3])-1;
				a=localization[a];
				b=localization[b];
				c=localization[c];
				triangles[3*currentIndex]=a;
				triangles[3*currentIndex+1]=b;
				triangles[3*currentIndex+2]=c;
				currentIndex++;
			}
		}

			//if (b<0) display.text=b.ToString();
			//if (c<0) display.text=c.ToString();

		/*for (int i=0; i<numberOfTetrahedrasInLayer [layer]; i++) {
			string[] sTemp = lines [i + indexTetrahedraLayer [layer]].Split (' ');
			int a = int.Parse(sTemp [1]) - 1;
			int b = int.Parse(sTemp [2]) - 1;
			int c = int.Parse(sTemp [3]) - 1;
			int d = int.Parse(sTemp [4]) - 1;
			if (a>=indexStartLayer2[layer]) a=a+indexStartLayer1[layer]-indexStartLayer2[layer];
			else a=a-indexStartLayer1[layer];
			if (b>=indexStartLayer2[layer]) b=b+indexStartLayer1[layer]-indexStartLayer2[layer];
			else b=b-indexStartLayer1[layer];
			if (c>=indexStartLayer2[layer]) c=c+indexStartLayer1[layer]-indexStartLayer2[layer];
			else c=c-indexStartLayer1[layer];
			if (d>=indexStartLayer2[layer]) d=d+indexStartLayer1[layer]-indexStartLayer2[layer];
			else d=d-indexStartLayer1[layer];

			if (a>=vertices.Length) display.text=a.ToString()+" "+(int.Parse(sTemp [1]) - 1).ToString ()+" "+(indexStartLayer2[layer]).ToString();;
			//if (b>=vertices.Length) display.text=b.ToString();
			//if (c>=vertices.Length) display.text=c.ToString();
			//if (d>=vertices.Length) display.text=d.ToString();
			
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i] = a;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 1] = b;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 2] = c;

			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 3] = b;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 4] = c;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 5] = d;

			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 6] = a;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 7] = b;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 8] = d;

			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 9] = a;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 10] = c;
			tri [3 * numberOfTrianglesInLayer [layer] + 3 * 4 * i + 11] = d;
		}*/

		
		//Getting the colors from the potentials file
		//potentials = data.text.Split ('\n');
		//colors = new Color[nbVertices];
		//colorBrain ();

		colors = new Color[nbVerticesInCurrentLayer];
		Color currentColor=new Vector4(0f,0f,1f,0.25f);
		if (color=="blue") currentColor=Color.blue;
		if (color=="black") currentColor=Color.black;
		if (color=="white") currentColor=Color.white;
		if (color=="red") currentColor=Color.red;
		if (color=="yellow") currentColor=Color.yellow;
		if (color=="grey") currentColor=Color.grey;
		if (color=="green") currentColor=Color.green;

		for (int i=0; i<nbVerticesInCurrentLayer; i++) {
			colors[i]=currentColor;
		}


		// Assigning the vertices, triangles and colors to the mesh
		mesh.vertices=vertices;
		mesh.triangles=triangles;
		mesh.colors = colors;
		
	}
	
	void Update() {
		// Updating the colors from a file in the Data sublocation of the executable
		// This only works on windows
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
		#endif
	}
	
	void colorBrain() { // Update of the colors in list "colors" with potentials value from list "potentials"
		
		// Getting the minimum and maximum of the values to have a better rendering of the colors
		float min = 0.0f;
		float max = 0.0f;
		for (int i=0; i<nbVertices; i++) {
			s = potentials [i];
			f = float.Parse (s);
			if (f >= max)
				max = f;
			if (f <= min)
				min = f;
		}
		if (max + min < 0)
			max = -min;
		
		// Coloring, if positif then in red, else in blue, with a degradation with the absolute value
		float intensity = 1.0f;
		for (int i=0; i<nbVertices; i++) {
			s = potentials [i];
			f = float.Parse (s);
			if (f >= 0.0f) {
				//colors [i] = new Color (f / max * 2.0f, 0.0f, 0.0f, 1.0f);
				colors [i] = new Color (intensity,(1.0f-f / max) * intensity,(1.0f-f / max) * intensity, 10.0f);
			}
			else {
				//colors [i] = new Color (0.0f, 0.0f, f / min * 2.0f, 1.0f);
				colors [i] = new Color ((1.0f-f / -max) * intensity,(1.0f-f / -max) * intensity,intensity, 10.0f);
			}
		}
	}

	string deleteSpaces(string s) {
		for (int i=0; i<20; i++) {
			s.Replace ("  ", " ");
		}
		return(s);
	}
}
