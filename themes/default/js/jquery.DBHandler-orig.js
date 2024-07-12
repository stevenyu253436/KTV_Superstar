/*

Description: Database Handler for Web SQL Database(HTML5 Spec)

Author : xxkuo@Transtep.com 


Usage Sample:

		$.DBHandler.connect("TESTWEBDB", 200000, "THIS IS A WEBDB TEST");
		$.DBHandler.migration(1, function(tx){
			tx.executeSql('CREATE TABLE IF NOT EXISTS tbl_contents (id unique, type, path, timelength)');	
		});
		$.DBHandler.migration(2, function(tx){
			tx.executeSql("INSERT INTO tbl_contents (id, type, path, timelength) values(?,?,?,?)", ["1","1","1","1"]);
		});
		$.DBHandler.migration(3, function(tx){
			tx.executeSql("INSERT INTO tbl_contents (id, type, path, timelength) values(?,?,?,?)", ["2","2","2","2"]);
		});
		$.DBHandler.migration(4, function(tx){
			tx.executeSql("INSERT INTO tbl_contents (id, type, path, timelength) values(?,?,?,?)", ["3","3","3","3"]);
		});

		$.DBHandler.doMigration();

		$.DBHandler.transaction(function (tx) { 
			tx.executeSql("SELECT * FROM tbl_contents", [], function(tx, result){
				for (var i = 0, item = null; i < result.rows.length; i++) {
					item = result.rows.item(i);
					console.log();
					alert(item["id"] + " " + item["type"]);	
				}
			}, function(tx, message){
				alert(message);
			});
		});

*/

$.extend({
	DBHandler: {
		migrations : [],
		database : false,
		connect : function(dbname, size, desc){
			try {
			    if (window.openDatabase) {
				this.database = openDatabase(dbname, "", desc, size);
				if (!this.database)
				    	alert("無法開始本地資料庫，可能沒有空間。");
			    } else
				alert("此瀏灠器沒有支援Web Storage Database");
			} catch(err) { }
		},
		transaction: function(func){
			this.database.transaction(func);
		},
		migration: function(version, func){
			this.migrations[version] = func;
		},
		doMigrationItem: function(version){
			if($.DBHandler.migrations[version]){
			      $.DBHandler.database.changeVersion(this.database.version, String(version), function(t){
				$.DBHandler.migrations[version](t);
			      }, function(err){
				if(console.error) console.error("Error!: %o", err);
			      }, function(){
				$.DBHandler.doMigration(version+1);
			      });
			}
		},
		doMigration: function(){
			var initialVersion = parseInt(this.database.version) || 0;
			try {
				this.doMigrationItem(initialVersion+1);
			} catch(e) {
				if(console.error) console.error(e);
			}
		}
	}
})();




