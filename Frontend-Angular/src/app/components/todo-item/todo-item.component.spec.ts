import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { TodoItemComponent } from './todo-item.component';
import { TodoItemService } from '../../services/todo-item.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { TodoItem } from '../../models/todo-item.model';
import { FormsModule } from '@angular/forms'; 

describe('TodoItemComponent', () => {
  let component: TodoItemComponent;
  let fixture: ComponentFixture<TodoItemComponent>;
  let serviceMock: jasmine.SpyObj<TodoItemService>;

  beforeEach(async () => {
    // Mock the TodoItemService
    serviceMock = jasmine.createSpyObj('TodoItemService', ['getItems', 'addItem', 'updateItem']);

    await TestBed.configureTestingModule({
      declarations: [ TodoItemComponent ],
      imports: [ 
        HttpClientTestingModule,
        FormsModule
       ],
      providers: [
        { provide: TodoItemService, useValue: serviceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TodoItemComponent);
    component = fixture.componentInstance;
    // service = TestBed.inject(TodoItemService) as jasmine.SpyObj<TodoItemService>;
  });

  it('should create', () => {
    // Check if component is created
    expect(component).toBeTruthy();
  });

  it('should load items on init and sort them', () => {
    // Prepare data to be returned by the getItems method
    const mockItems: TodoItem[] = [
      { description: 'Zebra', isCompleted: false },
      { description: 'Apple', isCompleted: false }
    ];
    serviceMock.getItems.and.returnValue(of(mockItems));
    
    // Trigger ngOnInit which calls getItems
    fixture.detectChanges();

    // Check if service was called and items are sorted as expected
    expect(serviceMock.getItems).toHaveBeenCalled();
    expect(component.items[0].description).toEqual('Apple');
    expect(component.items[1].description).toEqual('Zebra');
  });

  it('should handle adding a new item and sorting the list', fakeAsync(() => {
      // Assume initial items
      component.items = [
        { id: 1, description: 'Apple', isCompleted: false },
        { id: 2, description: 'Zebra', isCompleted: false }
      ];

    // Setup user input
    component.description = 'Banana';
    const newItem: TodoItem = {description: 'Banana', isCompleted: false };

    serviceMock.addItem.and.returnValue(of(newItem));

    // Call handleAdd
    component.handleAdd();
    tick();
    console.log('Actual items:', component.items);
    // Check service interaction
    expect(serviceMock.addItem).toHaveBeenCalledWith({ description: 'Banana', isCompleted: false });
    // Check items array is updated and sorted
    serviceMock.getItems.and.returnValue(of([...component.items, newItem]));
    // fixture.detectChanges();
    expect(component.items.map(item => item.description)).toEqual(['Apple', 'Banana', 'Zebra']);
    // Check input is cleared
    expect(component.description).toBe('');
  }));

  it('should clear description on handleClear call', () => {
    // Set a description
    component.description = 'New Task';
    
    // Call handleClear
    component.handleClear();

    // Check if description was cleared
    expect(component.description).toBe('');
  });

  it('should handle marking an item as complete', () => {
    const item: TodoItem = { id: 1, description: 'Task', isCompleted: false };
    serviceMock.updateItem.and.returnValue(of({ ...item, isCompleted: true }));

    // Call handleMarkAsComplete
    component.handleMarkAsComplete(item);

    // Validate service was called with correct data
    expect(serviceMock.updateItem).toHaveBeenCalledWith(item.id, { ...item, isCompleted: true });
    // Ensure item's isCompleted is set to true
    expect(item.isCompleted).toBeTrue();
  });

  it('should not mark an item as complete if it is already completed', () => {
    const item: TodoItem = { id: 1, description: 'Task', isCompleted: true };

    // Call handleMarkAsComplete on an already completed item
    component.handleMarkAsComplete(item);

    // Service should not be called
    expect(serviceMock.updateItem).not.toHaveBeenCalled();
  });
});
