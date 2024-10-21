1. La prima volta va in eccezione perchè cerca il file Index.resx. Occorre rinominare uno dei due file (en o it) ed eseguire l'applicazione.
   Poi si può riportare il nome del file con la propria giusta estensione (em.resx e it.resx).
2. Correggere la path del repository dei documenti opportunamente in appsetting.<development>.json
3. Per ricreare il file packages.lock.json contenente le dipendenze del pacchetti npm lanciare il comando da Package Manager Console
		npm i --package-lock-only
4. Per risolvere problemi di dipendenza tra i moduli eseguire da Package Manager Console
		npm audit fix --force
5. Per installare l'ultima versione di npm ho lanciato il comando
		npm install -g npm
   da Package Manager Console, mentre ho scaricato node.js con il msi pacchetto e installato in c:\\program files\nodejs (folder di default)
6. Non si possono sistemare i campi senza una <input ...>  o con lo <span ...> perchè non vengono passati al BE generando un errore di Sistem.IO.Memory
7. Per compilare i sorgenti sass scss fare atsto destro sul file e scegliere Re-compile file (il file scss deve essere incluso nel progetto)
