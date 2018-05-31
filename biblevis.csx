var text = File.ReadAllText(@"../Docs/KJV.txt");
var verses = text.Split('\n');
var books = new Dictionary<string,Dictionary<int,int>>();

string GetBook(string verse) => verse.Split(' ')[0];
string GetChapter(string verse) => verse.Split(' ')[1].Split(':')[0];
string GetVerse(string verse) => verse.Split(' ')[1].Split(':')[1];

string GetText(string verse)
{
	return verse.Substring(verse.IndexOf(' ', verse.IndexOf(':')))
				.Replace("\n", string.Empty)
				.Replace("\r", string.Empty)
				.Replace("[", string.Empty)
				.Replace("]", string.Empty)
				.Trim();
}

string GetVerseByRef(string book, int chapter, int verse)
{
	var reference = $"{book} {chapter}:{verse} ";
	return verses.Where(v => v.StartsWith(reference)).FirstOrDefault();
}

foreach (var verse in verses)
{
	string book = GetBook(verse);
	int chapter = int.Parse(GetChapter(verse));
	int verseNumber = int.Parse(GetVerse(verse));
	string text = GetText(verse);

	if (!books.ContainsKey(book)) 
	{
	 	books.Add(book, new Dictionary<int,int>());
	}
	
	books[book][chapter] = verseNumber;
}

StreamWriter json = File.AppendText(@"bible.json");
json.Write($@"{{""name"": ""Bible"", ""description"": ""KJV Bible"",");
json.Write($@"""children"": [");

var index = 0;
// Console.Write(string.Join(",", books.Keys.ToArray()));
// return;

foreach (var entry in books)
{
	var book = entry.Key;
	var chapters = entry.Value.Keys.Last();
	index++;
	json.Write($@"{{""name"":""{book}"", ""description"": ""{book}"", ""index"":{index}, ""size"":{chapters}, ""children"": [");

	for (var chapter = 1; chapter <= chapters; chapter++)
	{
		int verses = books[book][chapter];
		json.Write($@"{{""name"":{chapter}, ""description"": ""{chapter}"", ""index"":{chapter}, ""size"":{verses}, ""children"": [");

		for (var verse = 1; verse <= verses; verse++)
		{
			var text = GetText(GetVerseByRef(book, chapter, verse));
			json.Write($@"{{""name"":{verse}, ""description"": ""{text}"", ""index"":{verse}, ""size"":{text.Length}}}");
			if (verse != verses) json.Write(",");
		}

		json.Write($@"]}}");
		if (chapter != chapters) json.Write(",");
	}

	json.Write($@"]}}");
	if (book != books.Keys.Last()) json.Write(",");
}

json.Write($@"]}}");
json.Close();
json.Dispose();
Console.WriteLine("Done");
