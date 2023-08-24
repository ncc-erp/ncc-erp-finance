import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditOutcomingTypeComponent } from './edit-outcoming-type.component';

describe('EditOutcomingTypeComponent', () => {
  let component: EditOutcomingTypeComponent;
  let fixture: ComponentFixture<EditOutcomingTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditOutcomingTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditOutcomingTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
