(function($){
	$.amq = {
		connect : function(){
			$.amq._listeners = {};
			$.amq.connected = false;
			$.amq._service = org.activemq.Amq;
			$.amq.subscribe();
			try{
			        $.amq._service.init({
			                uri: '/amq',
			                logging: true,
		        	        timeout: 45,
		                	clientId: (new Date()).getTime().toString(),
					connectStatusHandler : function(connected){
						//console.info("amq connected:" + connected);
						if (!$.amq.connected && connected){
							$.amq._putListeners();
							$.amq.connected = true;
						}else if(!connected){
							$.amq.connected = false;
						}
					},
					sessionInitializedCallback: function(){
						console.info("amq ready.................");
						$.amq._putListeners();
						$.amq.connected = true;
					}
			        });
			}catch(e){
				console.info(e);
			}
		},
		subscribe : function(dest, callback){
			if (this.connected)
				$.amq._addListener(dest, callback);
			if (typeof(this._listeners[dest]) == "undefined"){
				this._listeners[dest] = [];
			}
			this._listeners[dest].push(callback);
		},
		unsubscribe : function(dest){
			 $.amq._service.removeListener('example', dest);
		},
		send : function(dest, message){
			if ($.amq.connected){
				$.amq._service.sendMessage(dest, message);
			}
		},
		connected : false,
		_service : null,
		_putListeners : function(){
			for(var k in $.amq._listeners){
				for(var i=0; i<$.amq._listeners[k].length; i++){
					$.amq._addListener(k, $.amq._listeners[k][i]);
				}
			}
		},
		_listeners : {},
		_addListener: function(dest, callback){
			//console.info("add subscribe:" + dest + "\n" + callback);
		        $.amq._service.addListener('example', dest, callback);
		}
	}
})(jQuery);

