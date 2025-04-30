from rest_framework import permissions

class IsLibrarian(permissions.BasePermission):
    """Allows access only to librarian users"""
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == 'LIBRARIAN'

class IsAdmin(permissions.BasePermission):
    """Allows access only to admin users"""
    def has_permission(self, request, view):
        return request.user.is_authenticated and (request.user.role == 'ADMIN' or request.user.is_superuser)

class IsPatron(permissions.BasePermission):
    """Allows access only to patron users"""
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == 'PATRON'

class IsStaffOrAdmin(permissions.BasePermission):
    """Allows access to staff users or admins"""
    def has_permission(self, request, view):
        return request.user.is_authenticated and (request.user.is_staff or request.user.role == 'ADMIN')