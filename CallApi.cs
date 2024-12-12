private void epiButtonC1_Click(object sender, System.EventArgs args)
{
	//API Key is included in the query param in this example. 
	var request = (HttpWebRequest)WebRequest.Create("https://server/Epicor10DEV/api/v2/efx/Company/TestLibrary/function-1/?api-key=yourAPIKeyHere");
	request.Method = "POST";
	//All REST v2 requests also sent with authentication method (Token, Basic)
	//This should be Base64 encoded
	string username = "username";
	string password = "password";
	string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
	request.Headers.Add("Authorization", "Basic " + encoded);
	
	//Add body to correspond to request signature
	request.ContentType = "application/json";
	using(var writer = new StreamWriter(request.GetRequestStream()))
	{
	var values = new Dictionary<string, string>
		{
		{"toEmailAddress", "toEmail@email.com"},
		{"fromEmailAddress","fromEmail@email.com"}, 
		{"body","This is the body"},   
		{"subject","Hello from Client Code!"}
		};
	string json = JsonConvert.SerializeObject(values);
	writer.Write(json);	
	}	
	using (var response = request.GetResponse()) 
	using (var reader = new StreamReader(response.GetResponseStream()))
	{
		var result = reader.ReadToEnd();
		epiTextBoxC1.Text = result.ToString();
	}

}
