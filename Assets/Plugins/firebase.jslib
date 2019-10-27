mergeInto(LibraryManager.library, {

  GetScoreData: function (path, responseFunction) {
    var ref = firebase.database().ref();
    var pathString = Pointer_stringify(path);
    var responseFunctionString = Pointer_stringify(responseFunction);

    var highscores = ref.child(pathString);

    highscores.orderByChild('score').limitToFirst(5).once('value', function (snapshot) {
      var obj = new Array();
      snapshot.forEach(function (data) {
        var scoreData = data.val();
        obj.push(scoreData);
      });

      SendMessage('JSListener', responseFunctionString, JSON.stringify(obj));

    });
  },

  UpdateHighscore: function (path, newscore) {
    var ref = firebase.database().ref();
    var pathString = Pointer_stringify(path);
    var userRef = ref.child(pathString +'/'+ discgolfValley.loggedInUserID);

    userRef.once("value", function (snapshot) {
      console.log(snapshot.val());

      if (snapshot.val() == null) {
        console.log("new user");
        userRef.set({
          score: newscore,
          userName: discgolfValley.displayName
        });
      }
      else {
        var currentscore = snapshot.child('score').val();
        console.log("current score: " + currentscore);

        if (currentscore > newscore) {
          userRef.set({
            score: newscore,
            userName: discgolfValley.displayName
          });
        }
      }
    });
  },


    WebUpdatePlayerData: function (playerData) {
      console.log("Saving Data");

      var ref = firebase.database().ref();
      var playerDataRef = ref.child('playerData/' + discgolfValley.loggedInUserID + '/playerProfile/');
      var playerDataString = Pointer_stringify(playerData);
      var jsonObject = JSON.parse(playerDataString)
      playerDataRef.set(jsonObject);
    },

    WebSaveHoleScore: function (guid, newscore, holeID) {
      console.log("Saving Hole Score");
      var guidString = Pointer_stringify(guid);
      var holeIDString = Pointer_stringify(holeID);

      var ref = firebase.database().ref();
      var holeRef = ref.child('playerData/' + discgolfValley.loggedInUserID + '/scores/' + holeIDString);

      holeRef.push({
        score: newscore,
        roundID: guidString
      });
    },

    WebGetHoleScore: function (holeID) {
      var holeIDString = Pointer_stringify(holeID);
      var ref = firebase.database().ref();
      var holeRef = ref.child('playerData/' + discgolfValley.loggedInUserID + '/scores/' + holeIDString);

      holeRef.once("value", function (snapshot) {
        console.log("Loading Data");
        if (snapshot.val() != null) {
          const hist = [];
          snapshot.forEach(function (data) {
            var score = data.child('score').val();
            hist.push(score);
          });

          const histy = hist.join(",");
          SendMessage('JSListener', 'GetHoleDataResponse', histy);
        }
        else {
          console.log("nooooo holeData");
        }
      });
    },

    WebGetOpenTournament: function () {
      var ref = firebase.database().ref();
      var tournamentRef = ref.child('tournaments/');
      tournamentRef.orderByChild('isCurrent').equalTo('true').once("value", function (snapshot) {
        if (snapshot.val() != null) {
          snapshot.forEach(function (data) {
            SendMessage('JSListener', 'GetOpenTournamentResponse', JSON.stringify(data.val()));
          });
        }
        else {
          console.log("nooooo running open tournament");
        }
      });
    },

    IsNewPlayer: function () {
      var ref = firebase.database().ref();
      var playerDataRef = ref.child('playerData/' + discgolfValley.loggedInUserID);

      playerDataRef.once("value", function (snapshot) {
        //console.log(snapshot.val());

        if (snapshot.val() != null) {
          //console.log("got playerdata");
          SendMessage('JSListener', 'NewPlayerResponse', 1);
        }
        else {
          //console.log("nooooo playerdata");
          SendMessage('JSListener', 'NewPlayerResponse', 2);
        }
      });
    },

    WebLoadPlayerData: function () {
      var ref = firebase.database().ref();
      var playerDataRef = ref.child('playerData/' + discgolfValley.loggedInUserID + '/playerProfile/');

      playerDataRef.once("value", function (snapshot) {
        //console.log(snapshot.val());
        console.log("Loading Data");

        if (snapshot.val() != null) {
          console.log("got playerdata");
          SendMessage('JSListener', 'GetPlayerDataResponse', JSON.stringify(snapshot.val()));
          //SendMessage('JSListener', 'GetPlayerDataResponse', snapshot.val().data);
        }
        else {
          console.log("nooooo playerdata");
        }
      });
    },

    WebPlayerAuthenticatedCheck: function () {
      var user = firebase.auth().currentUser;
      if (user) {

        console.log("user is authenticated");
        SendMessage('JSListener', 'PlayerAuthenticated', discgolfValley.displayName);
      } else {
        console.log("user is not authenticated");
        // No user is signed in.
      }
    },

  });

