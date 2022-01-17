/* Kod skapad av Simon Andersson för projektet i kursen DT071G, Programmering i C#.NET */

// Funktionalitet för JSON
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

        // När appen startar
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

            // Dölj etiketten med instruktion för att radera post
            label4.Text = "";

            // Rensa Data Source (ta bort koppling till listan)
            listBox1.DataSource = null;

            // Rensa listan före utskrift
            listBox1.Items.Clear();

            // Skapa ny instans av klassen Posthandle
            var Posthandle = new Posthandle(this); // ta bort och flytta till klassen posthandle?

            // Hämta alla poster
            var posts = Posthandle.getPosts();

            // Vänd på listan för att skriva ut senaste överst
            posts.Reverse();

            // Kontrollera om längden på listan understiger 5
            int reps;

            if(posts.Count < 5)
            {
                reps = posts.Count;
            } else
            {
                reps = 5;
            }

            // Hämta och loopa ut de senaste 5 posterna?
            for (int i = 0; i < reps; i++)
            {
                // Skriv ut Datum, typ och resultat på varsina rader
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

            // Ange instruktioner för att radera post
            label4.Text = "Dubbelklicka på en post för att radera";

            // Skapa ny instans av klassen Posthandle
            var Posthandle = new Posthandle(this);

            // Hämta alla poster
            var posts = Posthandle.getPosts();

            // Vänd på arrayen för att skriva ut senaste överst
            posts.Reverse();

            // Ange värde för varje post till dess unika Id
            listBox1.ValueMember = "Id";

            // Visa postens sammansatta värde (All)
            listBox1.DisplayMember = "All";

            // Bind samman listboxen med listan posts
            listBox1.DataSource = posts;

            // Ange att ingen post ska vara markerad
            listBox1.ClearSelected();
        }

        // Vid klick på knappen för att spara nytt framsteg
        public void button1_Click(object sender, EventArgs e)
        {
            // Hämta inmatad data och lagra som variabler
            var workouttype = textBox1.Text;
            var workoutdate = dateTimePicker1.Text;
            var workoutresult = textBox2.Text;

            // Skapa nytt objekt av klassen Posthandle
            var Posthandle = new Posthandle(this);

            // Anropa metod för att skapa ny post, skicka med inmatad data
            Posthandle.addPost(workouttype, workoutdate, workoutresult);
        }

        // Vid klick på knappen för att visa alla poster
        private void button2_Click(object sender, EventArgs e)
        {
            // Återställ meddelande
            label8.Text = "";

            // Om alla framsteg visas, gå tillbaka till "startsidan"
            if(label3.Text == "Alla framsteg")
            {
                // Metoden för att skriva ut de fem senaste framstegen
                printPosts();
                // Ändra text på knappen
                button2.Text = "Visa alla";
            }
            // Annars, visa alla framsteg
            else
            {
                // Metoden för att skriva ut alla framsteg
                printAllPosts();
                // Ändra text på knappen
                button2.Text = "Tillbaka";
            }
        }

        // Vid dubbelklick på post i listan
        private void doubleClicked(object sender, EventArgs e)
        {
            // Hämta medskickat objekt och lagra som variabel
            ListBox senderLB = (ListBox)sender;

            // Hämta id från klickat objekt i listan, konvertera till sträng
            string itemId = Convert.ToString(senderLB.SelectedValue);

            // Skapa nytt objekt från klassen Posthandle
            var Posthandle = new Posthandle(this);

            // Anropa metoden för att radera posten med aktuellt id
            Posthandle.deletePost(itemId);
        }

        // Autogenererade eventhanterare som ej används----------
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void label8_Click(object sender, EventArgs e){}
        // ------------------------------------------------------
    }

    // Klass för enskild post
    public class Post
    {
        // Attribut för objektet (träningstyp, datum, resultat, id och en sammanslagning)
        public string? Type { get; set; }
        public string? Date { get; set; }
        public string? Result { get; set; }
        public string? Id { get; set; }
        public string All { get { return Date + "  -  " + Type + "  -  " + Result; } }    
    }

    // Klass för att hantera poster
    public class Posthandle
    {
        // Fält för referens till Form 1
        private Form1 form;

        // Lagra JSON-fil som variabel
        private string file = @"posts.json";

        // Skapa lista för alla poster
        private List<Post> posts = new List<Post>();

        // Konstruerare, hämta listan från JSON-filen
        public Posthandle(Form1 form1)
        {
            // Tilldela objektet Form 1 till variablen form för åtkomst från denna klassen
            form = form1;

            // Läs in JSON-filen och gör om till array
            if (File.Exists(@"posts.json") == true)
            {
                string jsonString = File.ReadAllText(file);
                posts = JsonSerializer.Deserialize<List<Post>>(jsonString);
            }
        }

        // Metod för att lagra ny post
        public void addPost(string inputtype, string inputdate, string inputresult)
        {
            // Kontrollera att inmatad data ej är tom
            if(String.IsNullOrEmpty(inputtype) || String.IsNullOrEmpty(inputdate) || String.IsNullOrEmpty(inputresult))
            {
                // Skriv ut felmeddelande och stoppa metoden
                form.label8.ForeColor = Color.Red;
                form.label8.Text = "Alla fält måste fyllas i";
                return;
            }

            // Skapa objekt från klassen post
            var post = new Post();

            // Lagra medskickade variabler i objektet post
            post.Type = inputtype;
            post.Date = inputdate;
            post.Result = inputresult;

            // Lägg till id i form av datum, tid och millisekunder
            post.Id = DateTime.Now.ToString("MMddyyyyhhmmss.ffffff");

            // Lägg till nya posten till listan
            posts.Add(post);

            // Anropa metoden för att skriva ut datan till JSON-filen
            write();

            // Om alla framsteg visas, gå tillbaka till "startsidan"
            if (form.label3.Text == "Alla framsteg")
            {
                // Metoden för att skriva ut alla framsteg
                form.printAllPosts();
            }
            // Annars, visa alla framsteg
            else
            {
                // Metoden för att skriva ut de fem senaste framstegen
                form.printPosts();
            }

            // Rensa inmatningsformuläret
            form.textBox1.Text = "";
            form.textBox2.Text = "";

            // Skriv ut att lagringen lyckades
            form.label8.ForeColor = Color.MediumSeaGreen;
            form.label8.Text = "Framsteget sparades!";
        }

        // Metod för att hämta alla poster
        public List<Post> getPosts()
        {
            // Returnerar listan med alla poster
            return posts;
        }

        // Metod för att radera post
        public void deletePost(string id)
        {
            // Hitta index för posten med medskickat id
            string Id = id; // Hämta medskickat id
            int index = posts.FindIndex(x => x.Id == Id);
          
            // Om ett index hittades (ej lika med -1)
            if(index != -1)
            {
                // Radera posten med aktuellt index från listan
                posts.RemoveAt(index);

                // Uppdatera listan till JSON-filen
                write();

                // Skriv ut listan på nytt
                form.printAllPosts();
            }
        }

        // Metod för att skriva ut aktuell lista till JSON-filen
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