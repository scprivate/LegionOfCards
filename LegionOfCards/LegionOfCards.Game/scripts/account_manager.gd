extends Node

var _backend = null
var _session_manager = null
var _cache = null
var _manager = null
var _data_callback = null

func _init(manager, backend, session_manager):
	_manager = manager
	_backend = backend
	_session_manager = session_manager
	_backend.register("creation_data-check_result", funcref(self, "_on_creation_check_result"))
	_backend.register("account-creation_result", funcref(self, "_on_creation_result"))
	_backend.register("send-discord_verify_url", funcref(self, "_on_discord_verify_url_received"))
	_backend.register("account-deletion_result", funcref(self, "_on_deletion_result"))
	_backend.register("send_user-data", funcref(self, "_on_user_data_received"))
	
func request_user_data(callback):
	_data_callback = callback
	_backend.call_remote("request_user-data", [_session_manager.get_session_token()])
	
func _on_user_data_received(args):
	if args[0]:
		_data_callback.call_func(args[1], args[2], args[3])
		_data_callback = null
	else:
		_manager.open_popup("Fehler!", "Ein unerwarteter Fehler ist aufgetreten, bitte melde dich bei unserem Support!")
	
func _on_creation_check_result(args):
	var result_code = args[0]
	if result_code == 0:
		_backend.call_remote("create-account", [_cache["username"], _cache["email"], _cache["password"]])
		_cache = null
	elif result_code == 1:
		_manager.open_popup("Fehler!", "Die angegebene Email wurde bereits registriert!")
	elif result_code == 2:
		_manager.open_popup("Fehler!", "Der angegebene Benutzername wurde bereits registriert!")
	elif result_code == 3:
		_manager.open_popup("Fehler!", "Der angegebene Benutzername und die angebene Email wurden bereits registriert!")
	else:
		_manager.open_popup("Fehler!", "Ein unerwarteter Fehler ist aufgetreten, bitte melde dich bei unserem Support!")

func _on_creation_result(success):
	if success:
		_manager.clear_create_input()
		_manager.show_menu("login")
		_manager.open_popup("Erfolgreich erstellt!", "Der Account wurde erfolgreich erstellt!")
	else:
		_manager.open_popup("Fehler!", "Ein unerwarteter Fehler ist aufgetreten, bitte melde dich bei unserem Support!")
	
func _on_discord_verify_url_received(url):
	OS.shell_open(url)
	
func _on_deletion_result(code):
	if code == -1:
		_manager.open_popup("Fehler!", "Ein unerwarteter Fehler ist aufgetreten, bitte melde dich bei unserem Support!")
	elif code == 0:
		_session_manager.delete_session()
		_manager.open_popup("Erfolgreich gelöscht!", "Dein Account und alle Daten die mit diesem Account\nverbunden sind, wurden erfolgreich gelöscht!")
	else:
		_manager.open_popup("Bestätigung fehlgeschlagen!", "Das eingebene Passwort ist falsch und somit ist die Löschung nicht möglich!")

func create_account(username, email, password):
	_cache = {}
	_cache["username"] = username
	_cache["email"] = email
	_cache["password"] = password
	_backend.call_remote("check-creation_data", [username, email])
	
func verify_discord():
	if _session_manager.is_session_open():
		_backend.call_remote("request-discord_verification", _session_manager.get_session_token())

func delete_account(approved):
	if _session_manager.is_session_open():
		_backend.call_remote("delete-account", [_session_manager.get_session_token(), approved])