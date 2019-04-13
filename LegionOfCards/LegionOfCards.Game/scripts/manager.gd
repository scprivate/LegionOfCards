extends Node2D

const BLOCS_NEEDS_LOGIN = 0
const BLOCS_FOUND_LOGIN = 1
const BLOCS_CONNECTION_CLOSED = -1

var Utils = preload("res://scripts/utils.gd")
var Backend = load("res://scripts/backend.gd")
var SessionManager = load("res://scripts/session_manager.gd")
var AccountManager = load("res://scripts/account_manager.gd")

var backend = null
var session_manager = null
var account_manager = null

var _state = 0
var _current_menu = null
var _menus = {}

func _ready():
	_menus["login"] = Menu.new("login").add("lgn_identifier_input").add("lgn_password_input").add("lgn_login_button").add("lgn_create_account_button")
	_menus["register"] = Menu.new("register").add("rgr_password_input").add("rgr_password_input2").add("rgr_email_input").add("rgr_username_input").add("rgr_register_button").add("rgr_login_button")	
	_menus["main"] = Menu.new("main").add("mnm_logout_button").add("mnm_account_button")
	_menus["account"] = Menu.new("account").add("acc_back_button").add("acc_verify_button").add("acc_delete_button") 
	
	backend = Backend.new()
	session_manager = SessionManager.new(self, backend)
	account_manager = AccountManager.new(self, backend, session_manager)
	
	backend.register("ping_result", funcref(self, "_on_ping_result"))
	backend.register("connection-established", funcref(self, "_on_connection_established"))
	backend.register("disconnected", funcref(self, "_on_disconnected"))
	backend.register("force-disconnect", funcref(self, "_on_force_disconnect"))
	backend.init()
	
	# Login Menu
	get_node("lgn_login_button").connect("pressed", self, "_on_lgn_login_button_pressed")
	get_node("lgn_create_account_button").connect("pressed", self, "_on_lgn_register_button_pressed")
	# Register Menu
	get_node("rgr_login_button").connect("pressed", self, "_on_rgr_login_button_pressed")
	get_node("rgr_register_button").connect("pressed", self, "_on_rgr_register_button_pressed")
	# Main Menu
	get_node("mnm_logout_button").connect("pressed", self, "_on_mnm_logout_button_pressed")
	get_node("mnm_account_button").connect("pressed", self, "_on_mnm_account_button_pressed")
	# Account Menu
	get_node("acc_back_button").connect("pressed", self, "_on_acc_back_button_pressed")
	get_node("acc_back_button").connect("pressed", self, "_on_acc_back_button_pressed")
	
	for menu in _menus:
		hide_menu(_menus[menu])
	return

func _process(delta):
	backend.poll()
	
################## MENU EVENTS ##################

func _on_acc_verify_button_pressed():
	account_manager.verify_discord()

func _on_acc_back_button_pressed():
	show_menu("main")

func _on_mnm_account_button_pressed():
	account_manager.request_user_data(funcref(self, "_on_account_open_data"))
	
func _on_account_open_data(username, email, discord_success):
	if discord_success:
		$acc_verify_button.disabled = true
	else:
		$acc_verify_button.disabled = false
	show_menu("account")

func _on_mnm_logout_button_pressed():
	session_manager.request_logout()
	
func _on_rgr_login_button_pressed():
	show_menu("login")

func _on_rgr_register_button_pressed():
	$rgr_username_input.modulate = Color.white;
	$rgr_email_input.modulate = Color.white;
	$rgr_password_input2.modulate = Color.white;
	$rgr_password_input.modulate = Color.white;
	var username = $rgr_username_input.text
	var email = $rgr_email_input.text
	var password = $rgr_password_input.text
	var retry_password = $rgr_password_input2.text
	
	var cancel = false
	
	if Utils.is_null_or_empty(username):
		cancel = true
		$rgr_username_input.modulate = Color.red;
	if Utils.is_null_or_empty(email) or not "@" in email or not "." in email:
		cancel = true
		$rgr_email_input.modulate = Color.red;
	if Utils.is_null_or_empty(password):
		cancel = true
		$rgr_password_input.modulate = Color.red;
	if Utils.is_null_or_empty(retry_password) or retry_password != password:
		cancel = true
		$rgr_password_input2.modulate = Color.red;
	
	if not cancel:
		account_manager.create_account(username, email, password)
	
func _on_lgn_register_button_pressed():
	show_menu("register")
	
func _on_lgn_login_button_pressed():
	session_manager.request_session(get_node("lgn_identifier_input").text, get_node("lgn_password_input").text, funcref(self, "_on_login_result"))
	
################## MENU EVENTS ##################
	
func clear_create_input():
	$rgr_username_input.text = ""
	$rgr_email_input.text = ""
	$rgr_password_input.text = ""
	$rgr_password_input2.text = ""
	
func _on_login_result(success):
	if success:
		show_menu("main")
		get_node("lgn_identifier_input").text = "" 
		get_node("lgn_password_input").text = ""
		open_popup("Eingeloggt!", "Anmeldung erfolgreich!")	
	else:
		open_popup("Fehler!", "Die angegebenen Daten stimmen nicht überein!")	
	
	
func _on_connection_established():
	backend.call_remote("ping_session", [session_manager.get_session_token()])
	
func _on_force_disconnect():
	backend.disconnect_from_server()

func _on_ping_result(args):
	if args[0] == true:
		_state = BLOCS_FOUND_LOGIN
		session_manager.register_session(args[2])
		show_menu("main")
	else:
		session_manager.destroy_session()
		
func _on_disconnected():
	_state = BLOCS_CONNECTION_CLOSED
	set_process(false)
	hide_current_menu()
	open_popup("Verbindung verloren!", "Die Verbindung zu den Legion of Cards-Dedicated-Servern (Blocs) wurde abgebrochen! \nWährend keine Verbindung steht, kannst du Legion of Cards nicht spielen! \nVersuche es später erneut!")
	
func get_blocs_state():
	return _state
	
func set_state(state):
	_state = state
	
func get_backend():
	return backend
	
func get_session_manager():
	return session_manager
	
func hide_current_menu():
	if _current_menu != null:
		hide_menu(_current_menu)
		_current_menu = null
		
func hide_menu(menu):
	for control in menu.get_controls():
		toggle_control(control, false)

func open_popup(title, content):
	$menu_popup.window_title = title
	$menu_popup.dialog_text = content
	$menu_popup.popup_centered()
	return

func show_menu(name):
	hide_current_menu()
	if _menus.has(name):
		var menu = _menus[name]
		for control in menu.get_controls():
			toggle_control(control)
		_current_menu = menu
			
func toggle_control(name, visibility = true):
	var node = get_node(name)
	if node != null:
		node.visible = visibility
	
class Menu:
	var _controls = []
	var _name = null
	
	func _init(name):
		_name = name
		
	func get_name():
		return _name
	
	func add(control):
		_controls.append(control)
		return self
	
	func get_controls():
		return _controls