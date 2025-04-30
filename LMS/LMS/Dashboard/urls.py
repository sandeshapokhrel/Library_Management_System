from django.urls import path
from .views import DashboardView, GetOverdueBorrowersView

urlpatterns = [
    path('dashboards/', DashboardView.as_view(), name='dashboard'),
    path('dashboard/GetOverdueBorrowers', GetOverdueBorrowersView.as_view(), 
         name='overdue-borrowers'),
]
