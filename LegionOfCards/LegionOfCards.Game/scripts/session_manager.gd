extends Node

var Storage = load("res://scripts/storage.gd")
var storage = null

var _manager = null
var _backend = null
var _access_token = null

var _login_callback = null

func _init(manager, backend):
	storage = Storage.new()
	
	_manager = manager
	_backend = backend
	var data = storage.load_storage()
	if data != null and data.has("access_token"):
		_access_token = data.access_token
	
	backend.register("session-result", funcref(self, "_on_session_result"))
	backend.register("session-destroyed", funcref(self, "_on_session_destroyed"))
	
func _on_session_destroyed():
	destroy_session()
	
func _on_session_result(args):
	if args[0] == true:
		register_session(args[2])
	else:
		destroy_session()
	
	if _login_callback != null:
		_login_callback.call_func(args[0])
		_login_callback = null
		
func register_session(token):
	_access_token = token
	var data = {}
	data.access_token = _access_token
	storage.save_storage(data)

func is_session_open():
	if _access_token != null:
		return true
	else:
		return false
		
func request_session(identifier, password, callback):
	_login_callback = callback
	_backend.call_remote("check-password", [identifier, password])
	
func destroy_session():
	_access_token = null
	var data = {}
	storage.save_storage(data)
	if _manager.get_blocs_state() == _manager.BLOCS_CONNECTION_CLOSED:
		pass
	else:
		_manager.set_state(_manager.BLOCS_NEEDS_LOGIN)
		_manager.show_menu("login")
	
func request_logout():
	if is_session_open():
		_backend.call_remote("logout-session", _access_token)
		
func get_session_token():
	return _access_token