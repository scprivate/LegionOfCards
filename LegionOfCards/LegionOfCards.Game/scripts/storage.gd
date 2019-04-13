extends Node

const PATH = "res://storage.json"

func load_storage():
	var stream = File.new()
	if not stream.file_exists(PATH):
		return null
		
	if stream.open(PATH, File.READ) != 0:
		return null
	var data = stream.get_as_text()
	stream.close()
	return JSON.parse(data).result
	
func save_storage(data):
	var stream = File.new()
	stream.open(PATH, File.WRITE)
	stream.store_line(to_json(data))
	stream.close()
	return true
	
func free_persist():
	var save_nodes = get_tree().get_nodes_in_group("persistent")
	for i in save_nodes:
		i.queue_free()