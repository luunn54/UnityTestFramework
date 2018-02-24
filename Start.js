const http = require('http');

function getRndInteger(min, max) {
    return Math.floor(Math.random() * (max - min + 1) ) + min;
}

function checkWin(board, x, y) {
	var win = false;

	var val = board[x][y];
	if(board[x][0] == val && board[x][1] == val && board[x][2] == val){
		win = true;
	}else if(board[0][y] == val && board[1][y] == val && board[2][y] == val){
		win = true;
	}else if(board[0][0] == val && board[1][1] == val && board[2][2] == val){
		win = true;
	}else if(board[2][0] == val && board[1][1] == val && board[0][2] == val){
		win = true;
	}

	return win;
}

var USERS = {
	'luu' : {
		'name' : 'Nguyen Ngoc Luu',
		'pw' : '1234',
		'diamond' : 68
	},

	'thao' : {
		'name' : 'Nguyen Van Thao',
		'pw' : '7890',
		'diamond' : 30
	}
}

ROMS = {

}

ROMS['luu'] = { 'diamond' : 10, "board" : [[0, 0, 0], [0, 0, 0], [0, 0, 0]] }


http.createServer((request, response) => {
  const { headers, method, url } = request;
  let body = [];
  request.on('error', (err) => {
    console.error(err);
  }).on('data', (chunk) => {
    body.push(chunk);
  }).on('end', () => {
    body = Buffer.concat(body).toString();
    // BEGINNING OF NEW STUFF

    response.on('error', (err) => {
      console.error(err);
    });

	response.writeHead(200, {'Content-Type': 'application/json'})

    var status = 200;
    var data = {}
    
    var request_data = JSON.parse(body);

    var user = USERS[request_data.user];

	if(user != undefined){
	    if (url === '/login') {
			if(user.pw == request_data.pw){
	    		data.user = user;
	    	}else{
	    		status = 400;
	    		data.msg = 'password wrong!';	
	    	}
	    }else if (url === '/create_room') {
	    	var diamond = request_data.diamond;
	    	user.diamond -= diamond;
	    	ROMS[request_data.user] = { 'diamond' : diamond, "board" : [[0, 0, 0], [0, 0, 0], [0, 0, 0]] }
	    	data.user = user;
	    }else if (url === '/set') {
	    	var room = ROMS[request_data.user];
	    	if(room != undefined){
		    	var x = request_data.x;
		    	var y = request_data.y;

		    	var board = room.board;
		    	var current = board[x][y];

		    	// data.current = current;

		    	// console.error(current + "->" + board[x][y])

		    	if(current == 0){
		    		board[x][y] = 1;
		    		var win = checkWin(board, x, y);
		    		
		    		if (win) {
		    			data.game_status = 'YOU_WIN';
		    			user.diamond += room.diamond * 2;
		    		}else{
		    			for (var i = 0; i < 300; i++) {
		    				x = getRndInteger(0, 2);
		    				y = getRndInteger(0, 2);
		    				
		    				if(board[x][y] == 0){
		    					break;
		    				}
		    			}

	    				if(board[x][y] != 0){
		    				data.game_status = 'DRAW';
		    				user.diamond += room.diamond;
		    			}else{
		    				board[x][y] = 2;
		    				data.x = x;
		    				data.y = y;
		    				win = checkWin(board, x, y);
		    				if(win){
		    					data.game_status = 'BOT_WIN';	
		    				}else{
		    					data.game_status = 'PLAYING';	
		    				}
		    			}
		    		}

		    		for (var i = 0; i < 3; i++) {
		    			console.error(board[i][0] + " " + board[i][1] + " " + board[0][2]);
		    		}

		    		data.user = user;
		    	}else{
		    		status = 500;
    				data.msg = 'can not set this cell!';
		    	}
	    	}else{
	    		status = 401;
    			data.msg = 'room not found!';		
	    	}
	    	
	    }else{

	    }
    }else{
    	status = 401;
    	data.msg = 'user not found!';
    }

	const responseBody = { status, data };

    response.write(JSON.stringify(responseBody));
    response.end();
    // Note: the 2 lines above could be replaced with this next one:
    // response.end(JSON.stringify(responseBody))

    // END OF NEW STUFF
  });
}).listen(8080);