import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditNoteNRevenueComponent } from './edit-note-n-revenue.component';

describe('EditNoteNRevenueComponent', () => {
  let component: EditNoteNRevenueComponent;
  let fixture: ComponentFixture<EditNoteNRevenueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditNoteNRevenueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditNoteNRevenueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
