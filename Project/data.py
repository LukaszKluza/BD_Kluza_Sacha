import random
from datetime import datetime, timedelta

marks = ["Toyota", "Toyota", "Toyota", "Toyota", "Toyota", "Toyota", "BMW", "BMW", "BMW", "BMW", "BMW", "Audi", "Audi", "Audi", "Audi", "Audi", "Opel", "Opel", "Opel", "Opel"]
models = ["Corolla", "Auris", "Avensis", "RAV4", "Prius", "CHR", "M3", "M5", "X3", "X5", "X7", "A3", "A4", "A6", "Q3", "Q5", "Corsa", "Astra", "Insignia", "Grandland X"]
names = ["John", "Alice", "Michael", "Emily", "David", "Emma", "James", "Olivia", "William", "Sophia", "Daniel", "Isabella", "Matthew", "Mia", "Alexander", "Charlotte", "Benjamin", "Amelia", "Henry", "Ella"]
surenames = ["Wick", "Johnson", "Smith", "Brown", "Wilson", "Martinez", "Anderson", "Garcia", "Jones", "Hernandez", "Gonzalez", "Perez", "Rodriguez", "Lopez", "Hill", "Scott", "Green", "Baker", "Nelson", "Carter"]
prices = [130, 100, 140, 150, 90, 130, 300, 400, 180, 190, 250, 110, 130, 150, 140, 150, 95, 120, 140, 150]
insurance_type = ["full", "standard", "partial", "none"]

def main():
    print("[")
    for i in range(12, 21):
        print("{")
        print('"_id": {},'.format(i))
        car = random.randint(0, 19)
        print('"rental_car": {')
        print('  "carId": {},'.format(car))
        print('  "make": "{}",'.format(marks[car]))
        print('  "model": "{}",'.format(models[car]))
        print('  "price_per_day": {}'.format(prices[car]))
        print("},")
        client = random.randint(0, 19)
        print('"customer": {')
        print('  "clientId": {},'.format(client))
        print('  "first_name": "{}",'.format(names[client]))
        print('  "last_name": "{}",'.format(surenames[client]))
        print("},")
        start_date = datetime.now()
        days_to_add_for_expected_end = random.randint(1, 10)
        expected_end_date = start_date + timedelta(days=days_to_add_for_expected_end)
        days_to_add_for_end = random.randint(0, 3)
        end_date = expected_end_date + timedelta(days=days_to_add_for_end)
        days_between = (end_date - start_date).days
        days_between_expected = (expected_end_date - start_date).days
        start_date_str = start_date.isoformat()
        expected_end_date_str = expected_end_date.isoformat()
        end_date_str = end_date.isoformat()
        print('"rental_details": {')
        print('  "start_date": {')
        print('    "$date": "{}"'.format(start_date_str))
        print('  },')
        print('  "expected_end_date": {')
        print('    "$date": "{}"'.format(expected_end_date_str))
        print('  },')
        print('  "end_date": {')
        print('    "$date": "{}"'.format(end_date_str))
        print('  },')
        print('  "rental_status": "finished",')
        insurance = random.randint(0, 3)
        print('  "insurance_type": "{}",'.format(insurance_type[insurance]))
        print('  "extra_insurance_amount": {},'.format(0))
        print('  "days": {},'.format(days_between))
        value = (days_between - days_between_expected)*(prices[car] + 10)
        print('  "extra_days_amount": {},'.format(value))
        print('  "mileage:": {},'.format(days_between*150))
        print('  "extra_mileage_amount": {},'.format(0))
        print('  "extra_fuel": {},'.format(0))
        print('  "extra_fuel_amount": {},'.format(0))
        price = days_between_expected*prices[car]
        print('  "price": {},'. format(price))
        print('  "discount": {},'.format(0))
        print('  "extra_amount": {},'.format(value))
        print('  "final_amount": {}'.format(value+price))
        print("}")
        print("},")
    print("]")

main()