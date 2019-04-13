extends Node

static func is_null_or_empty(variant):
	if variant == null or typeof(variant) != TYPE_STRING:
		return true
	return variant.empty()