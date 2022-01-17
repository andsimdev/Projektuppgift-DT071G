/* Kod skapad av Simon Andersson f�r projektet i kursen DT071G, Programmering i C#.NET */

// Funktionalitet f�r JSON
using System.Text.Json;
using System;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // N�r appen startar
        private void Form1_Load(object sender, EventArgs e)
        {
            // Skapa instans av klassen posthandle med referens till denna klass
            Posthandle posthandle = new Posthandle(this);

            // Skriv ut alla poster till sidan
            printPosts();
        }

        // Skriv ut 5 senaste posterna
        public void printPosts()
        {
            // Ange rubrik
            label3.Text = "Fem senaste framstegen";

            // D�lj etiketten med instruktion f�r att radera post
            label4.Text = "";

            // Rensa Data Source (ta bort koppling till listan)
            listBox1.DataSource = null;

            // Rensa listan f�re utskrift
            listBox1.Items.Clear();

            // Skapa ny instans av klassen Posthandle
            var Posthandle = new Posthandle(this); // ta bort och flytta till klassen posthandle?

            // H�mta alla poster
            var posts = Posthandle.getPosts();

            // V�nd p� listan f�r att skriva ut senaste �verst
            posts.Reverse();

            // Kontrollera om l�ngden p� listan understiger 5
            int reps;

            if(posts.Count < 5)
            {
                reps = posts.Count;
            } else
            {
                reps = 5;
            }

            // H�mta och loopa ut de senaste 5 posterna?
            for (int i = 0; i < reps; i++)
            {
                // Skriv ut Datum, typ och resultat p� varsina rader
                listBox1.Items.Add(posts[i].Date);
                listBox1.Items.Add(posts[i].Type);
                listBox1.Items.Add(posts[i].Result);
                listBox1.Items.Add("\n");
            }
        }

        // Skriv ut alla poster
        public void printAllPosts()
        {
            // Ange rubrik
            label3.Text = "Alla framsteg";

            // Ange instruktioner f�r att radera post
            label4.Text = "Dubbelklicka p� en post f�r att radera";

            // Skapa ny instans av klassen Posthandle
            var Posthandle = new Posthandle(this);

            // H�mta alla poster
            var posts = Posthandle.getPosts();

            // V�nd p� arrayen f�r att skriva ut senaste �verst
            posts.Reverse();

            // Ange v�rde f�r varje post till dess unika Id
            listBox1.ValueMember = "Id";

            // Visa postens sammansatta v�rde (All)
            listBox1.DisplayMember = "All";

            // Bind samman listboxen med listan posts
            listBox1.DataSource = posts;

            // Ange att ingen post ska vara markerad
            listBox1.ClearSelected();
        }

        // Vid klick p� knappen f�r att spara nytt framsteg
        public void button1_Click(object sender, EventArgs e)
        {
            // H�mta inmatad data och lagra som variabler
            var workouttype = textBox1.Text;
            var workoutdate = dateTimePicker1.Text;
            var workoutresult = textBox2.Text;

            // Skapa nytt objekt av klassen Posthandle
            var Posthandle = new Posthandle(this);

            // Anropa metod f�r att skapa ny post, skicka med inmatad data
            Posthandle.addPost(workouttype, workoutdate, workoutresult);
        }

        // Vid klick p� knappen f�r att visa alla poster
        private void button2_Click(object sender, EventArgs e)
        {
            // �terst�ll meddelande
            label8.Text = "";

            // Om alla framsteg visas, g� tillbaka till "startsidan"
            if(label3.Text == "Alla framsteg")
            {
                // Metoden f�r att skriva ut de fem senaste framstegen
                printPosts();
                // �ndra text p� knappen
                button2.Text = "Visa alla";
            }
            // Annars, visa alla framsteg
            else
            {
                // Metoden f�r att skriva ut alla framsteg
                printAllPosts();
                // �ndra text p� knappen
                button2.Text = "Tillbaka";
            }
        }

        // Vid dubbelklick p� post i listan
        private void doubleClicked(object sender, EventArgs e)
        {
            // H�mta medskickat objekt och lagra som variabel
            ListBox senderLB = (ListBox)sender;

            // H�mta id fr�n klickat objekt i listan, konvertera till str�ng
            string itemId = Convert.ToString(senderLB.SelectedValue);

            // Skapa nytt objekt fr�n klassen Posthandle
            var Posthandle = new Posthandle(this);

            // Anropa metoden f�r att radera posten med aktuellt id
            Posthandle.deletePost(itemId);
        }

        // Autogenererade eventhanterare som ej anv�nds----------
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void label8_Click(object sender, EventArgs e){}
        // ------------------------------------------------------
    }

    // Klass f�r enskild post
    public class Post
    {
        // Attribut f�r objektet (tr�ningstyp, datum, resultat, id och en sammanslagning)
        public string? Type { get; set; }
        public string? Date { get; set; }
        public string? Result { get; set; }
        public string? Id { get; set; }
        public string All { get { return Date + "  -  " + Type + "  -  " + Result; } }    
    }

    // Klass f�r att hantera poster
    public class Posthandle
    {
        // F�lt f�r referens till Form 1
        private Form1 form;

        // Lagra JSON-fil som variabel
        private string file = @"posts.json";

        // Skapa lista f�r alla poster
        private List<Post> posts = new List<Post>();

        // Konstruerare, h�mta listan fr�n JSON-filen
        public Posthandle(Form1 form1)
        {
            // Tilldela objektet Form 1 till variablen form f�r �tkomst fr�n denna klassen
            form = form1;

            // L�s in JSON-filen och g�r om till array
            if (File.Exists(@"posts.json") == true)
            {
                string jsonString = File.ReadAllText(file);
                posts = JsonSerializer.Deserialize<List<Post>>(jsonString);
            }
        }

        // Metod f�r att lagra ny post
        public void addPost(string inputtype, string inputdate, string inputresult)
        {
            // Kontrollera att inmatad data ej �r tom
            if(String.IsNullOrEmpty(inputtype) || String.IsNullOrEmpty(inputdate) || String.IsNullOrEmpty(inputresult))
            {
                // Skriv ut felmeddelande och stoppa metoden
                form.label8.ForeColor = Color.Red;
                form.label8.Text = "Alla f�lt m�ste fyllas i";
                return;
            }

            // Skapa objekt fr�n klassen post
            var post = new Post();

            // Lagra medskickade variabler i objektet post
            post.Type = inputtype;
            post.Date = inputdate;
            post.Result = inputresult;

            // L�gg till id i form av datum, tid och millisekunder
            post.Id = DateTime.Now.ToString("MMddyyyyhhmmss.ffffff");

            // L�gg till nya posten till listan
            posts.Add(post);

            // Anropa metoden f�r att skriva ut datan till JSON-filen
            write();

            // Om alla framsteg visas, g� tillbaka till "startsidan"
            if (form.label3.Text == "Alla framsteg")
            {
                // Metoden f�r att skriva ut alla framsteg
                form.printAllPosts();
            }
            // Annars, visa alla framsteg
            else
            {
                // Metoden f�r att skriva ut de fem senaste framstegen
                form.printPosts();
            }

            // Rensa inmatningsformul�ret
            form.textBox1.Text = "";
            form.textBox2.Text = "";

            // Skriv ut att lagringen lyckades
            form.label8.ForeColor = Color.MediumSeaGreen;
            form.label8.Text = "Framsteget sparades!";
        }

        // Metod f�r att h�mta alla poster
        public List<Post> getPosts()
        {
            // Returnerar listan med alla poster
            return posts;
        }

        // Metod f�r att radera post
        public void deletePost(string id)
        {
            // Hitta index f�r posten med medskickat id
            string Id = id; // H�mta medskickat id
            int index = posts.FindIndex(x => x.Id == Id);
          
            // Om ett index hittades (ej lika med -1)
            if(index != -1)
            {
                // Radera posten med aktuellt index fr�n listan
                posts.RemoveAt(index);

                // Uppdatera listan till JSON-filen
                write();

                // Skriv ut listan p� nytt
                form.printAllPosts();
            }
        }

        // Metod f�r att skriva ut aktuell lista till JSON-filen
        private void write()
        {
            // Ange indenterad JSON-kod
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Serialisera listan till JSON
            var json = JsonSerializer.Serialize(posts, options);

            // Skriv till JSON-filen
            File.WriteAllText(file, json);
        }
    }
}