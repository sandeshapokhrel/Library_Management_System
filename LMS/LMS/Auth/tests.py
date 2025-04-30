from django.test import TestCase
from rest_framework.test import APIClient
from rest_framework import status
from django.urls import reverse
from .models import User
from .permissions import IsLibrarian, IsAdmin, IsPatron

class AuthTests(TestCase):
    def setUp(self):
        self.client = APIClient()
        self.admin = User.objects.create_user(
            user_name='admin',
            password='adminpass',
            role='ADMIN',
            is_staff=True
        )
        self.librarian = User.objects.create_user(
            user_name='librarian',
            password='libpass',
            role='LIBRARIAN'
        )
        self.patron = User.objects.create_user(
            user_name='patron',
            password='patronpass',
            role='PATRON'
        )

    def test_user_registration(self):
        url = reverse('register_user')
        data = {'user_name': 'newuser', 'password': 'testpass123'}
        response = self.client.post(url, data, format='json')
        self.assertEqual(response.status_code, status.HTTP_201_CREATED)
        self.assertEqual(User.objects.count(), 4)
        new_user = User.objects.get(user_name='newuser')
        self.assertEqual(new_user.role, 'PATRON')

    def test_admin_access(self):
        self.client.force_authenticate(user=self.admin)
        response = self.client.get('/api/admin-route/')
        self.assertNotEqual(response.status_code, status.HTTP_403_FORBIDDEN)

    def test_librarian_permissions(self):
        self.client.force_authenticate(user=self.librarian)
        response = self.client.get('/api/librarian-route/')
        self.assertNotEqual(response.status_code, status.HTTP_403_FORBIDDEN)

    def test_patron_restrictions(self):
        self.client.force_authenticate(user=self.patron)
        response = self.client.get('/api/admin-route/')
        self.assertEqual(response.status_code, status.HTTP_403_FORBIDDEN)

    def test_jwt_authentication(self):
        url = reverse('token_obtain_pair')
        data = {'user_name': 'patron', 'password': 'patronpass'}
        response = self.client.post(url, data, format='json')
        self.assertEqual(response.status_code, status.HTTP_200_OK)
        self.assertIn('access', response.data)
        self.assertIn('refresh', response.data)

class PermissionTests(TestCase):
    def test_librarian_permission(self):
        user = User(role='LIBRARIAN')
        permission = IsLibrarian()
        self.assertTrue(permission.has_permission(None, None, user))

    def test_admin_permission(self):
        user = User(role='ADMIN')
        permission = IsAdmin()
        self.assertTrue(permission.has_permission(None, None, user))

    def test_patron_permission(self):
        user = User(role='PATRON')
        permission = IsPatron()
        self.assertTrue(permission.has_permission(None, None, user))
