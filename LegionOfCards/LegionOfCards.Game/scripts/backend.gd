extends Node

var events = {}
var client = null;
var connection_check = false

func init():
	print("Trying to connect...")
	client = StreamPeerTCP.new()
	client.connect_to_host("127.0.0.1", 25352)
	
func register(name, callback):
	events[name] = callback
	
func poll():
	if !connection_check and client.get_status() == StreamPeerTCP.STATUS_CONNECTED:
		connection_check = true
		
	if connection_check:
		var bytes = client.get_available_bytes()
		if bytes > 0:
			var message = client.get_string(bytes)
			var json = JSON.parse(message).result
			handle_event(json.Key, json.Args)

func call_remote(key, args):
	var packet = {}
	packet.Key = key
	packet.Args = args
	var json = to_json(packet)
	client.put_string (json)

func handle_event(key, args):
	if events.has(key):
		if args.size() == 0:
			events[key].call_func()
		else:
			events[key].call_func(args)
			
func get_conn_status():
	return client.get_status()
	
func disconnect_from_server():
	client.disconnect_from_host()
	handle_event("disconnected", [])