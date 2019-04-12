extends Node2D

var Backend = load("res://scripts/backend.gd")
var backend

func _ready():
	get_node("login_button").connect("pressed", self, "_on_login_button_pressed")
	backend = Backend.new()
	backend.register("ping_result", funcref(self, "_on_ping_result"))
	backend.register("connection-established", funcref(self, "_on_connection_established"))
	backend.register("disconnected", funcref(self, "_on_disconnected"))
	backend.register("force-disconnect", funcref(self, "_on_force_disconnect"))
	backend.init()
	return

func _process(delta):
	backend.poll()
	
func _on_login_button_pressed():
	print("Login wurde gedrückt!")
	
func _on_connection_established():
	backend.call_remote("ping_session", ["cookie"])
	
func _on_force_disconnect():
	backend.disconnect_from_server()

func _on_ping_result(args):
	if args[0] == true:
		print("Session registering: user: %s ; token: %s" % [args[1], args[2]])
	else:
		print("Destroying session")
		
func _on_disconnected():
	print("Disconnected")
	set_process(false)