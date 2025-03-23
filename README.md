# RandomPicker
Simple application for choosing a random video from playlists on YouTube.
<details>
<summary>How to launch:</summary>

1. Download any version from releases: randompicker-&lt;version&gt;.zip
2. Unpack to any place.
3. For the work of the program, you need to get YouTube API Key:

	* Go to https://console.cloud.google.com;
	* Click "Select project"/"New project":
	* Write 'Project Name' and click the "Create" button;
	* Click "API &amp; Services", go to the "Libraries" section;
	* In the search, enter "YouTube Data API V3", select "YouTube Data API V3";
	* In the opened window, press the "Enable" button;
	* Go to the "Credentials" section, press "Create Credentials"/"Api Key";
	* Copy the generated key.

4. In the "Config" folder, open the "Settings.json" file (any text editor) and replace the value in the ApiKey line with the generated key.
5. In the "Config" folder, open the "Urls.json" file (any text editor) and insert links to any playlists on YouTube.com.
6. Run the program.
</details>
<details>
<summary>Для запуска:</summary>
  
1. Скачать любую версию из релизов: randompicker-&lt;version&gt;.zip
2. Разархивировать в любое место.
3. Для работы программы необходимо получить Youtube API Key:
   
	* перейти на https://console.cloud.google.com
	* Нажать "Select project"/"New project";
	* Заполнить поле "Project name" и нажать кнопку "Create";
	* нажать "API & services", перейти в раздел "Libraries";
	* в поиске ввести "youtube data api v3", выбрать "YouTube Data API v3";
	* в открывшемся окне нажать кнопку "Enable";
	* перейти в раздел "Credentials", нажать "Create credentials"/"Api key";
	* скопировать сгенерированный ключ

5. В папке "Config" открыть (любым текстовым редактором) файл "Settings.json" и заменить значение в строке ApiKey на полученный ключ.
6. В папке "Config" открыть (любым текстовым редактором) файл "Urls.json" и вставить ссылки на любые плейлисты с youtube.com.
7. Запустить программу.
</details>
