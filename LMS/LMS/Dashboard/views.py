from django.shortcuts import render
from rest_framework.views import APIView
from rest_framework.response import Response
from rest_framework.permissions import IsAuthenticated

from .serializers import OverdueBorrowerSerializer

from .models import OverdueBorrower
from .services import DashboardService

class DashboardView(APIView):
    
    permission_classes = [IsAuthenticated]

    def get(self, request):
        dashboard_data = DashboardService.get_dashboard_data()
        return Response(dashboard_data)
    
class GetOverdueBorrowersView(APIView):
    permission_classes = [IsAuthenticated]

    def get(self, request):
        borrowers = OverdueBorrower.objects.all()
        serializer = OverdueBorrowerSerializer(borrowers, many=True)
        return Response(serializer.data)
    
    def get(self, request):
        service = DashboardService()
        overdue_borrowers = service.get_overdue_borrowers()
        serializer = OverdueBorrowerSerializer(overdue_borrowers, many=True)
        return Response(serializer.data)
    
    def get(self, request):
        service = DashboardService()
        overdue_borrowers = service.get_overdue_borrowers()  # Emails are sent here
        serializer = OverdueBorrowerSerializer(overdue_borrowers, many=True)
        return Response({"message": "Emails sent to overdue borrowers", "data": serializer.data})